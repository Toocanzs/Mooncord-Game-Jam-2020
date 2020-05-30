using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : MonoBehaviour
{

    public Tilemap OccluderTilemap;
    public static PathfindingManager Instance;
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
    void Update()
    {
        OccluderTilemap.GetTilesBlock(OccluderTilemap.cellBounds);
    }
}

public static class TilemapExtensions
{
    //this.GetTileAssetsBlock(bounds.min, bounds.size)
}