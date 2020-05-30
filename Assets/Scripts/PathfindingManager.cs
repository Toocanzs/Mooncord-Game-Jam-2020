using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingManager : MonoBehaviour
{

    public Tilemap OccluderTilemap;
    public static PathfindingManager Instance;

    private float timeSinceLastGrabbedTiles = 0;
    public float tileUpdateInterval = 0.25f;

    struct TilePathfindingData
    {
        public int2 closestSeen;
        public int distance;
        public bool occupied;
    }
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

    }

    void Update()
    {
        
        if (PlayerCharacter.Instance != null)
        {
        }
    }
    
    /*
     NativeArray<TilePathfindingData> output = new NativeArray<TilePathfindingData>(tileData.Length, Allocator.TempJob);
            var job = new UpdatePathfindingJob
            {
                TileData = tileData,
                bounds = OccluderTilemap.cellBounds,
                Output = output,
                PlayerPosition = PlayerCharacter.Instance.transform.position
            };
            var handle = job.Schedule(tileData.Length, 16);
            handle.Complete();
            tileData.CopyFrom(output);
            output.Dispose();
     
     static int2 PositionFromIndex(int index, BoundsInt bounds)
    {
        int x = index % bounds.size.x;
        int y = index / bounds.size.x;
        return new int2(x, y);
    }
    static int IndexFromPosition(int2 pos, BoundsInt bounds)
    {
        return pos.y * bounds.size.x + pos.x;
    }
    
    struct UpdatePathfindingJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<TilePathfindingData> TileData;
        [WriteOnly]
        public NativeArray<TilePathfindingData> Output;

        public BoundsInt bounds;
        public float3 PlayerPosition;
        public void Execute(int index)
        {
            int2 pos = PositionFromIndex(index, bounds);


            int2 playerPos = (int2)math.floor(PlayerPosition.xy);
            int2 closestSeen = int.MaxValue;
            int smallestDist = int.MaxValue;
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    var newPos = pos + new int2(xOffset, yOffset);
                    if (math.any(new bool4(newPos < 0, newPos >= new int2(bounds.size.x, bounds.size.y))))
                        continue;
                    var data = TileData[IndexFromPosition(newPos, bounds)];
                    if (data.colliderType != Tile.ColliderType.Grid)
                    {
                        if (math.all(playerPos == data.closestSeen))
                        {
                            if (data.distance <= smallestDist)
                            {
                                smallestDist = data.distance;
                            }
                        }
                    }
                }
            }

            var current = TileData[index];
            current.distance = smallestDist + 1;
            current.closestSeen = closestSeen;
            Output[index] = current;
        }
    }*/
}