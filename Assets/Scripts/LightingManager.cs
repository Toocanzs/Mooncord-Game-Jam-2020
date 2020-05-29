using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public Camera lightingCamera;

    public int PixelsPerUnit = 32;
    public int RayCount = 32;
    public int2 ProbeCounts = new int2(50, 30);
    public float2 followPercent = new float2(0.3f, 0.3f);

    public RenderTexture WallBuffer;
    public RenderTexture LightingPerProbeBuffer;
    public DoubleBuffer LightingPerPixelBuffer;

    public static LightingManager Instance;
    public Material TransferToFullscreenMaterial;
    public Material TransferValidDataMaterial;

    public float hysteresis = 4;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError($"Two instances of {this} found. This is supposed to be a singleton");
            return;
        }

        Instance = this;
    }

    void Start()
    {
        transform.position = new Vector3(1000,1000,1000);//Move away so it corrects itself automatically
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
        LightingPerPixelBuffer.enableRandomWrite = true;
        LightingPerPixelBuffer.Create();
        
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
        lightingCamera.orthographicSize = (float) ProbeCounts.y / 2;
    }

    public void SetCenterRounded(float3 pos)
    {
        transform.position = math.floor(pos) + 0.5f;
    }

    public bool UpdateCameraPos()
    {
        float2 currentPos = ((float3)transform.position).xy;
        float2 lowerBounds = currentPos - (followPercent * ProbeCounts)/2;
        float2 upperBounds = currentPos + (followPercent * ProbeCounts)/2;
        float2 cameraPos = ((float3)Camera.main.transform.position).xy;
        if (math.any(new bool4(cameraPos < lowerBounds, cameraPos > upperBounds)))
        {
            SetCenterRounded(Camera.main.transform.position);
            return true;
        }

        return false;
    }

    public float3 getBottomLeft()
    {
        return transform.position - new Vector3((ProbeCounts.x / 2f), (ProbeCounts.y / 2f), 0);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(ProbeCounts.x, ProbeCounts.y, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(followPercent.x * ProbeCounts.x, followPercent.y * ProbeCounts.y, 0));

        Gizmos.color = Color.blue;
        for (int y = 0; y < ProbeCounts.y; y++)
        {
            for (int x = 0; x < ProbeCounts.x; x++)
            {
                Gizmos.DrawSphere(transform.position + new Vector3(x - ProbeCounts.x/2f, y  - ProbeCounts.y/2f, 0), 0.1f);
            }
        }
        
    }
}