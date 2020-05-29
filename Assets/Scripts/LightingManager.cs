using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Camera lightingCamera;

    public int PixelsPerUnit = 32;
    public int2 ProbeCounts = new int2(50, 30);
    public float2 followPercent = new float2(0.3f, 0.3f);
    
    public RenderTexture WallBuffer;
    public RenderTexture LightingPerProbeBuffer;
    public DoubleBuffer LightingPerPixelBuffer;
    
    public static LightingManager Instance;

    public virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError($"Two instances of {this} found. This is supposed to be a singleton");
            return;
        }

        Instance = this;
    }
    
    void Start()
    {
        lightingCamera = GetComponent<Camera>();
        
        WallBuffer = new RenderTexture(ProbeCounts.x * PixelsPerUnit, ProbeCounts.y * PixelsPerUnit, 0,
            RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
        WallBuffer.Create();
        
        LightingPerProbeBuffer = new RenderTexture(ProbeCounts.x, ProbeCounts.y, 0,
            RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear);
        LightingPerProbeBuffer.enableRandomWrite = true;
        LightingPerProbeBuffer.Create();
        
        LightingPerPixelBuffer = new RenderTexture(ProbeCounts.x * PixelsPerUnit, ProbeCounts.y * PixelsPerUnit, 0,
            RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.Linear).ToDoubleBuffer();
        LightingPerProbeBuffer.enableRandomWrite = true;
        LightingPerProbeBuffer.Create();
    }

    private void OnDestroy()
    {
        LightingPerProbeBuffer.Release();
        LightingPerPixelBuffer.Release();
        WallBuffer.Release();
    }
    

    void Update()
    {
        lightingCamera.aspect = (float) ProbeCounts.x / ProbeCounts.y;
        lightingCamera.orthographicSize = (float) ProbeCounts.x / 2;
    }
}
