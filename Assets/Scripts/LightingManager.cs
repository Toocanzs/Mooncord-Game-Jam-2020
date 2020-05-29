using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class LightingManager : Singleton<LightingManager>
{
    public Camera lightingCamera;

    public int PixelsPerUnit = 32;
    public int2 ProbeCounts = new int2(50, 30);
    void Start()
    {
        lightingCamera = GetComponent<Camera>();
    }
    void Update()
    {
        
    }
}
