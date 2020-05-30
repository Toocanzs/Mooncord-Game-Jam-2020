using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(LightingManager))]
public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager Instance;
    private LightingManager lightingManager;

    public int TestsPerFrame = 20;
    private int testIndex = 0;

    public LayerMask pathfindingBlockingLayers;

    private NativeArray<TilePathfindingData> probeData;

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

        lightingManager = GetComponent<LightingManager>();
        probeData = new NativeArray<TilePathfindingData>(lightingManager.totalProbes, Allocator.Persistent);
        for (int i = 0; i < lightingManager.totalProbes; i++)
        {
            probeData[i] = new TilePathfindingData
            {
                distance = int.MaxValue,
                closestSeen = int.MaxValue,
                occupied = false
            };
        }

        lightingManager.OnLightingProbesMoved += HandleProbesMoved;
    }

    private void HandleProbesMoved()
    {
        
    }

    private void OnDestroy()
    {
        probeData.Dispose();
    }

    void Update()
    {
        for (int i = 0; i < TestsPerFrame; i++)
        {
            float2 probePos = lightingManager.probeIndexToXy(testIndex);
            bool occupied = Physics2D.OverlapCircle(probePos, 0.3f, pathfindingBlockingLayers);
            
            //update data
            var currentData = probeData[testIndex];
            currentData.occupied = occupied;
            probeData[testIndex] = currentData; 

            testIndex = (testIndex + 1) % lightingManager.totalProbes;
        }

        float3 playerPosition = PlayerCharacter.Instance.transform.position;
        float2 playerPosRelative = (playerPosition - (float3)lightingManager.transform.position).xy;
        if (math.all(new bool4(playerPosRelative > 0, playerPosRelative < lightingManager.ProbeCounts)))
        {
            int index = IndexFromPosition((int2) playerPosRelative, lightingManager.ProbeCounts);
            var current = probeData[index];
            int2 playerPosRounded = (int2)playerPosition.xy;
            current.closestSeen = playerPosRounded;
            probeData[index] = current;
        }
        
        NativeArray<TilePathfindingData> output = new NativeArray<TilePathfindingData>(probeData.Length, Allocator.TempJob);
        var job = new UpdatePathfindingJob
        {
            TileData = probeData,
            ProbeCounts = lightingManager.ProbeCounts,
            Output = output,
            PlayerPosition = PlayerCharacter.Instance.transform.position
        };
        var handle = job.Schedule(probeData.Length, 16);
        handle.Complete();
        probeData.CopyFrom(output);
        output.Dispose();
    }

    static int2 PositionFromIndex(int index, int2 probeCounts)
    {
        int x = index % probeCounts.x;
        int y = index / probeCounts.x;
        return new int2(x, y);
    }
    static int IndexFromPosition(int2 pos, int2 probeCounts)
    {
        return pos.y * probeCounts.x + pos.x;
    }
    
    struct UpdatePathfindingJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<TilePathfindingData> TileData;
        [WriteOnly]
        public NativeArray<TilePathfindingData> Output;

        public int2 ProbeCounts;
        public float3 PlayerPosition;
        public void Execute(int index)
        {
            int2 pos = PositionFromIndex(index, ProbeCounts);


            int2 playerPos = (int2)PlayerPosition.xy;
            int2 closestSeen = int.MaxValue;
            int smallestDist = int.MaxValue;
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    var newPos = pos + new int2(xOffset, yOffset);
                    if (math.any(new bool4(newPos < 0, newPos >= new int2(ProbeCounts.x, ProbeCounts.y))))
                        continue;
                    var data = TileData[IndexFromPosition(newPos, ProbeCounts)];
                    if (!data.occupied)
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
    }
}