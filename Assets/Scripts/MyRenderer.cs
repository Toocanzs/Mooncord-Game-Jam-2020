﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MyRenderer : ScriptableRenderer
{
    public Lighting2DPass Lighting2DPass;

    public MyRenderer(ScriptableRendererData data) : base(data)
    {
        Lighting2DPass = new Lighting2DPass();
        RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
        RenderPipelineManager.beginFrameRendering += BeginFrameRendering;
    }

    private void BeginFrameRendering(ScriptableRenderContext arg1, Camera[] arg2)
    {
    }

    private void BeginCameraRendering(ScriptableRenderContext arg1, Camera arg2)
    {
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
        sortingLayers = SortingLayer.layers;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        Camera camera = renderingData.cameraData.camera;
        FilteringSettings filteringSettings = new FilteringSettings();
        filteringSettings.renderQueueRange = RenderQueueRange.all;
        filteringSettings.layerMask = -1;
        filteringSettings.renderingLayerMask = uint.MaxValue;
        filteringSettings.sortingLayerRange = SortingLayerRange.all;

        var identifier = colorAttachment;
        var tags = nonLightTags;

        var command = CommandBufferPool.Get("2dLighting");
        command.Clear();
        var drawingSettings = CreateDrawingSettings(tags, ref renderingData, SortingCriteria.CommonTransparent);

        for (int i = 0; i < sortingLayers.Length; i++)
        {
            short layerValue = (short) sortingLayers[i].value;
            short lowerBound = i == 0 ? short.MinValue : layerValue;
            short upperBound = i == sortingLayers.Length - 1 ? short.MaxValue : layerValue;
            filteringSettings.sortingLayerRange = new SortingLayerRange(lowerBound, upperBound);
            CoreUtils.SetRenderTarget(command, identifier, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store, ClearFlag.All, Color.black);
            context.ExecuteCommandBuffer(command);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
        }

        command.Release();
    }
}