using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : MonoBehaviour
{

    public Tilemap OccluderTilemap;
    public static PathfindingManager Instance;

    private float timeSinceLastGrabbedTiles = 0;
    public float tileUpdateInterval = 0.25f;
    private TileBase[] tiles;

    struct TilePathfindingData
    {
        public float2 closestSeen;
        public int distance;
    }

    private NativeArray<TilePathfindingData> tileData;
    void Start()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("Two Pathfinding managers exist in the scene. Deleting one");
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (tileData.IsCreated)
        {
            tileData.Dispose();
        }
    }

    void Update()
    {
        if (tiles == null || timeSinceLastGrabbedTiles > tileUpdateInterval)
        {
            tiles = OccluderTilemap.GetTilesBlock(OccluderTilemap.cellBounds);
            if (tiles.Length != tileData.Length)
            {
                if (tileData.IsCreated)
                {
                    tileData.Dispose();
                }
                tileData = new NativeArray<TilePathfindingData>(tiles.Length, Allocator.Persistent);
            }
            timeSinceLastGrabbedTiles = 0;
        }
       
        timeSinceLastGrabbedTiles += Time.deltaTime;
    }
}