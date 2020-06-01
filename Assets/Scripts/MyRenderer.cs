using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRenderer : ScriptableRenderer
{
    public Lighting2DPass Lighting2DPass;

    private static ComputeShader computeShader = Resources.Load<ComputeShader>("LightingShader");
    private int raycastKernel = computeShader.FindKernel("LightingRaycast");

    private float randomIndex = 0;

    public MyRenderer(ScriptableRendererData data) : base(data)
    {
        Lighting2DPass = new Lighting2DPass();
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
        RenderPipelineManager.beginFrameRendering += BeginFrameRendering;
        randomIndex = 0;
        if (computeShader == null)
            Debug.LogError("Unable to load compute shader");
    }

    ~MyRenderer()
    {
        RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
        RenderPipelineManager.beginFrameRendering -= BeginFrameRendering;
    }

    private void BeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        var manager = LightingManager.Instance;
        if (manager != null)
        {
            Shader.SetGlobalFloat("hysteresis", Time.deltaTime * LightingManager.Instance.hysteresis);
            randomIndex += manager.noiseMultiplier * Time.deltaTime;
            if (randomIndex > short.MaxValue)
                randomIndex = 0;
            
            float2 lastCameraPos = ((float3) LightingManager.Instance.lightingCamera.transform.position).xy;
            bool moved = LightingManager.Instance.UpdateCameraPos();
            float2 currentCameraPos = ((float3) LightingManager.Instance.lightingCamera.transform.position).xy;
                
            if (moved)
            {
                var command = CommandBufferPool.Get("2dLighting");
                command.Clear();
                float2 difference = currentCameraPos - lastCameraPos;
                float2 offset = difference / (manager.ProbeCounts);
                manager.TransferValidDataMaterial.SetVector("_Offset", offset.xyxy);
                command.Blit(manager.LightingPerPixelBuffer.Current, manager.LightingPerPixelBuffer.Other, manager.TransferValidDataMaterial);
                command.Blit(manager.WallBuffer.Current, manager.WallBuffer.Other, manager.TransferValidDataMaterial);
                context.ExecuteCommandBuffer(command);
                command.Clear();
                manager.LightingPerPixelBuffer.Swap();
                manager.WallBuffer.Swap();
                
                command.Clear();
                command.Release();
            }
        }
        
    }

    private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        var manager = LightingManager.Instance;
        var command = CommandBufferPool.Get("2dLighting");
        command.Clear();
        if (manager != null)
        {
            if (camera.TryGetComponent<LightingCameraTag>(out _))
            {
                command.SetComputeTextureParam(computeShader, raycastKernel, "WallBuffer", manager.WallBuffer.Current);
                command.SetComputeTextureParam(computeShader, raycastKernel, "LightingPerProbeBuffer", manager.LightingPerProbeBuffer);

                command.SetComputeIntParam(computeShader, "RayCount", manager.RayCount);
                command.SetComputeIntParam(computeShader, "PixelsPerUnit", manager.PixelsPerUnit);
                command.SetComputeIntParams(computeShader, "ProbeCounts", manager.ProbeCounts.x, manager.ProbeCounts.y);
                command.SetComputeVectorParam(computeShader, "LightingOrigin", LightingManager.Instance.getBottomLeft().xyxy);

                float2 randomProbeOffset = new float2(UnityEngine.Random.Range(-0.5f,0.5f), UnityEngine.Random.Range(-0.5f,0.5f));
                command.SetComputeVectorParam(computeShader, "RandomProbeOffset", randomProbeOffset.xyxy);
                
                float goldenRatio = (1f + math.sqrt(5)) / 2f;
                float randomRayOffset = randomIndex * 2f * math.PI * (goldenRatio - 1f);
                randomRayOffset %= (2f * math.PI) / manager.RayCount;
                command.SetComputeFloatParam(computeShader, "RandomRayOffset", randomRayOffset);
                command.DispatchCompute(computeShader, raycastKernel, (manager.ProbeCounts.x + 63) / 64, manager.ProbeCounts.y, 1);
                context.ExecuteCommandBuffer(command);
                command.Clear();
                
                manager.TransferToFullscreenMaterial.SetVector("_Offset", new float4(-randomProbeOffset / manager.ProbeCounts, 0, 0));
                manager.TransferToFullscreenMaterial.SetTexture("_PreviousFullScreenTex", manager.LightingPerPixelBuffer.Current);
                
                command.Blit(manager.LightingPerProbeBuffer, manager.LightingPerPixelBuffer.Other, manager.TransferToFullscreenMaterial);
                context.ExecuteCommandBuffer(command);
                command.Clear();
                
                manager.LightingPerPixelBuffer.Swap();

                
            }
            else
            {
                command.SetGlobalVector("LightingOrigin", LightingManager.Instance.getBottomLeft().xyxy);
                command.SetGlobalInt("PixelsPerUnit", manager.PixelsPerUnit);    
                command.SetGlobalVector("ProbeCounts", (float4)manager.ProbeCounts.xyxy);           
                command.SetGlobalTexture("_LightTexture", manager.LightingPerPixelBuffer.Current);
            }
        }


        context.ExecuteCommandBuffer(command);
        command.Clear();
        command.Release();
    }

    public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var colorIdentifier = RenderTargetHandle.CameraTarget.Identifier();
        ConfigureCameraTarget(colorIdentifier, BuiltinRenderTextureType.CameraTarget);
        Lighting2DPass.ConfigureTarget(colorIdentifier);
        EnqueuePass(Lighting2DPass);
    }
}

public class Lighting2DPass : ScriptableRenderPass
{
    public List<ShaderTagId> nonLightTags = new List<ShaderTagId>() {new ShaderTagId("Universal2D")};
    public List<ShaderTagId> lightTags = new List<ShaderTagId>() {new ShaderTagId("Lighting2D")};

    private SortingLayer[] sortingLayers;

    public Lighting2DPass()
    {
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        sortingLayers = SortingLayer.layers;
        Camera camera = renderingData.cameraData.camera;
        FilteringSettings filteringSettings = new FilteringSettings();
        filteringSettings.renderQueueRange = RenderQueueRange.all;
        filteringSettings.layerMask = -1;
        filteringSettings.renderingLayerMask = uint.MaxValue;
        filteringSettings.sortingLayerRange = SortingLayerRange.all;

        var identifier = colorAttachment;
        var tags = nonLightTags;

        var manager = LightingManager.Instance;

        if (camera.TryGetComponent<LightingCameraTag>(out _) && manager != null)
        {
            identifier = manager.WallBuffer.Current;
            tags = lightTags;
        }

        var command = CommandBufferPool.Get("2dLighting");
        command.Clear();
        var drawingSettings = CreateDrawingSettings(tags, ref renderingData, SortingCriteria.CommonTransparent);

        for (int i = 0; i < sortingLayers.Length; i++)
        {
            short layerValue = (short) sortingLayers[i].value;
            short lowerBound = i == 0 ? short.MinValue : layerValue;
            short upperBound = i == sortingLayers.Length - 1 ? short.MaxValue : layerValue;
            filteringSettings.sortingLayerRange = new SortingLayerRange(lowerBound, upperBound);
            CoreUtils.SetRenderTarget(command, identifier, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, ClearFlag.All, Color.clear);
            context.ExecuteCommandBuffer(command);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        command.Release();
    }
}