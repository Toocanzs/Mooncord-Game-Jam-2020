using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightingManager : Singleton<LightingManager>
{
    public Camera lightingCamera;

    public int PixelsPerUnit = 32;
    public int2 ProbeCounts = new int2(50, 30);

    public RenderTexture LightingPerProbeBuffer;
    public DoubleBuffer LightingPerPixelBuffer;
    void Start()
    {
        lightingCamera = GetComponent<Camera>();
        
        LightingPerProbeBuffer = new RenderTexture(ProbeCounts.x, ProbeCounts.y, 0,
            RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
        LightingPerProbeBuffer.enableRandomWrite = true;
        LightingPerProbeBuffer.Create();
        LightingPerPixelBuffer = new RenderTexture(ProbeCounts.x * PixelsPerUnit, ProbeCounts.y * PixelsPerUnit, 0,
            RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear).ToDoubleBuffer();
    }

    private void OnDestroy()
    {
        LightingPerProbeBuffer.Release();
        LightingPerPixelBuffer.Release();
    }
    

    void Update()
    {
        lightingCamera.aspect = (float) ProbeCounts.x / ProbeCounts.y;
        lightingCamera.orthographicSize = (float) ProbeCounts.x / 2;
    }
}
