﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Burst;
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
                distance = int.MaxValue - 1,
                closestSeen = int.MaxValue - 1,
                occupied = false
            };
        }

        lightingManager.OnLightingProbesMoved += HandleProbesMoved;
    }

    private void HandleProbesMoved(float3 delta)
    {
        var intDif = (int3)math.round(delta);
        //TODO: handle probe movement
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
            probeData[testIndex] = new TilePathfindingData
            {
                closestSeen = currentData.closestSeen,
                distance = currentData.distance,
                occupied = currentData.occupied
            }; 

            testIndex = (testIndex + 1) % lightingManager.totalProbes;
        }

        float3 playerPosition = PlayerCharacter.Instance.transform.position;
        float2 playerPosRelative = (playerPosition - lightingManager.getBottomLeft()).xy;
        if (math.all(new bool4(playerPosRelative > 0, playerPosRelative < lightingManager.ProbeCounts)))
        {
            int index = IndexFromPosition((int2) playerPosRelative, lightingManager.ProbeCounts);
            var current = probeData[index];
            int2 playerPosRounded = (int2)playerPosition.xy;
            probeData[index] = new TilePathfindingData
            {
                distance = 0,
                closestSeen = playerPosRounded,
                occupied = current.occupied
            };
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
        output.CopyTo(probeData);
        output.Dispose();
    }

    private void OnDrawGizmosSelected()
    {
        if (lightingManager != null)
        {
            for (int i = 0; i < lightingManager.totalProbes; i++)
            {
                int2 pos = PositionFromIndex(i, lightingManager.ProbeCounts);
                Vector3 gizmoPos = lightingManager.getBottomLeft() + new float3(pos.x, pos.y, 0);
                int dist = probeData[i].distance;
                Gizmos.color = new Color((float)dist/255, 0, 0);
                Gizmos.DrawSphere(gizmoPos, 0.4f);
            }
        }
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
    
    [BurstCompile]
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
            var current = TileData[index];

            int2 playerPos = (int2)PlayerPosition.xy;
            int2 closestSeen = int.MaxValue - 1;
            int smallestDist = int.MaxValue - 1;
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int2 newPos = pos + new int2(xOffset, yOffset);
                    if (math.any(new bool4(newPos < 0, newPos >= ProbeCounts)))
                        continue;
                    var data = TileData[IndexFromPosition(newPos, ProbeCounts)];
                    smallestDist = math.min(smallestDist, data.distance);
                    if (!data.occupied)
                    {
                        if (math.all(playerPos == data.closestSeen))
                        {
                            if (data.distance <= smallestDist)
                            {
                                smallestDist = data.distance;
                                closestSeen = playerPos;
                            }
                        }
                    }
                }
            }

            
            Output[index] = new TilePathfindingData
            {
                distance = smallestDist + 1,
                closestSeen = closestSeen,
                occupied = current.occupied
            };
        }
    }
}