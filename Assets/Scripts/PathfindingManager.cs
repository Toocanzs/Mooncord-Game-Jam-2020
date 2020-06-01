using System;
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
    private NativeArray<float2> directionData;

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
                distance = 99999,
                closestSeen = 99999,
                occupied = true
            };
        }
        
        directionData = new NativeArray<float2>(lightingManager.totalProbes, Allocator.Persistent);

        lightingManager.OnLightingProbesMoved += HandleProbesMoved;
    }

    private void HandleProbesMoved(float3 delta)
    {
        var intDif = (int3) math.round(delta);

        NativeArray<TilePathfindingData> output = new NativeArray<TilePathfindingData>(probeData.Length, Allocator.TempJob);
        TransferDataJob job = new TransferDataJob
        {
            ProbeCounts = lightingManager.ProbeCounts,
            Delta = intDif.xy,
            Output = output,
            TileData = probeData
        };
        job.Schedule(probeData.Length, 8).Complete();
        output.CopyTo(probeData);
        output.Dispose();

        UpdateDirections();
    }

    private void UpdateDirections()
    {
        CalculateDirectionsJob job = new CalculateDirectionsJob
        {
            Output = directionData,
            TileData = probeData,
            ProbeCounts = lightingManager.ProbeCounts
        };
        job.Schedule(probeData.Length, 8).Complete();
    }
    
    public float2 GetDirectionToPlayer(float3 worldPos)
    {
        float2 relativePos = (worldPos - lightingManager.getBottomLeft()).xy;
        int2 BL_pos = (int2) relativePos;
        int2 BR_pos = BL_pos + new int2(1, 0);
        int2 TL_pos = BL_pos + new int2(0, 1);
        int2 TR_pos = BL_pos + new int2(1, 1);

        float2 BL = GetDirInBounds(BL_pos);
        float2 BR = GetDirInBounds(BR_pos);
        float2 TL = GetDirInBounds(TL_pos);
        float2 TR = GetDirInBounds(TR_pos);

        float2 f = relativePos - BL_pos;
        float2 top = math.lerp(TL, TR, f.x);
        float2 bottom = math.lerp(BL, BR, f.x);

        float2 dir = math.lerp(bottom, top, f.y);

        if (math.dot(dir, dir) > 0.01)
        {
            dir = math.normalize(dir);
        }
        
        if (PositionInBounds(BL_pos, lightingManager.ProbeCounts))
        {
            var data = probeData[IndexFromPosition(BL_pos, lightingManager.ProbeCounts)];
            if (data.occupied)
            {
                float2 directionAway = math.normalize(relativePos - BL_pos);
                return math.normalize(dir + directionAway*30);
            }
        }
        return dir;
    }

    private float2 GetDirInBounds(int2 pos)
    {
        if (PositionInBounds(pos, lightingManager.ProbeCounts))
        {
            int index = IndexFromPosition(pos, lightingManager.ProbeCounts);
            return directionData[index];
        }

        return 0;
    }

    private static bool PositionInBounds(int2 newPos, int2 probeCounts)
    {
        if (math.any(new bool4(newPos < 0, newPos >= probeCounts)))
        {
            return false;
        }
        return true;
    }

    private void OnDestroy()
    {
        probeData.Dispose();
        directionData.Dispose();
        lightingManager.OnLightingProbesMoved -= HandleProbesMoved;
        Instance = null;
    }

    void Update()
    {
        for (int i = 0; i < TestsPerFrame; i++)
        {
            float2 probePos = lightingManager.probeIndexToXy(testIndex);
            bool occupied = Physics2D.OverlapCircle(probePos, 0.3f, pathfindingBlockingLayers);

            //update data
            var currentData = probeData[testIndex];
            probeData[testIndex] = new TilePathfindingData
            {
                closestSeen = currentData.closestSeen,
                distance = currentData.distance,
                occupied = occupied
            };
            
            testIndex = (testIndex + 1) % lightingManager.totalProbes;
        }

        float3 playerPosition = PlayerCharacter.GetPostion();
        float2 playerPosRelative = (playerPosition - lightingManager.getBottomLeft()).xy;
        if (math.all(new bool4(playerPosRelative > 0, playerPosRelative < lightingManager.ProbeCounts)))
        {
            int index = IndexFromPosition((int2) playerPosRelative, lightingManager.ProbeCounts);
            var current = probeData[index];
            int2 playerPosRounded = (int2) playerPosition.xy;
            probeData[index] = new TilePathfindingData
            {
                distance = 0,
                closestSeen = playerPosRounded,
                occupied = current.occupied
            };
        }

        NativeArray<TilePathfindingData> output = new NativeArray<TilePathfindingData>(probeData.Length, Allocator.TempJob);
        JobHandle dependency = default(JobHandle);

            var job = new UpdatePathfindingJob
            {
                TileData = probeData,
                ProbeCounts = lightingManager.ProbeCounts,
                Output = output,
                PlayerPosition = PlayerCharacter.GetPostion()
            };
            var handle = job.Schedule(probeData.Length, 8, dependency);
            dependency = handle;

        dependency.Complete();
                
        output.CopyTo(probeData);
        output.Dispose();

        UpdateDirections();
    }

    private void OnDrawGizmosSelected()
    {
        if (lightingManager != null)
        {
            for (int i = 0; i < lightingManager.totalProbes; i++)
            {
                int2 pos = PositionFromIndex(i, lightingManager.ProbeCounts);
                Vector3 gizmoPos = lightingManager.getBottomLeft() + new float3(pos.x, pos.y, 0);
                int dist = probeData[i].distance * 5;
                Gizmos.color = new Color((float) dist / 255, probeData[i].occupied ? 255 : 0, 0);
                //Gizmos.DrawSphere(gizmoPos, 0.4f);
                float2 dir = GetDirectionToPlayer(gizmoPos);
                Gizmos.DrawRay(gizmoPos, new Vector3(dir.x, dir.y, 0));
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

    [BurstCompile(CompileSynchronously = true)]
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

            int2 playerPos = (int2) PlayerPosition.xy;
            int2 closestSeen = 99999;
            int smallestDist = 99999;
            if (!current.occupied)
            {
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
            }


            Output[index] = new TilePathfindingData
            {
                distance = math.min(smallestDist + 1, 99999),
                closestSeen = closestSeen,
                occupied = current.occupied
            };
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    struct TransferDataJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<TilePathfindingData> TileData;

        [WriteOnly]
        public NativeArray<TilePathfindingData> Output;

        public int2 ProbeCounts;

        public int2 Delta;

        public void Execute(int index)
        {
            int2 pos = PositionFromIndex(index, ProbeCounts);
            int2 offsetPos = pos + Delta;
            if (math.any(new bool4(offsetPos < 0, offsetPos >= ProbeCounts)))
            {
                Output[index] = new TilePathfindingData
                {
                    closestSeen = 99999,
                    distance = 99999,
                    occupied = true
                };
            }
            else
            {
                Output[index] = TileData[IndexFromPosition(offsetPos, ProbeCounts)];
            }
        }
    }
    
    [BurstCompile(CompileSynchronously = true)]
    struct CalculateDirectionsJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<TilePathfindingData> TileData;

        [WriteOnly]
        public NativeArray<float2> Output;

        public int2 ProbeCounts;

        public void Execute(int index)
        {
            var currentPositionData = TileData[index];
            if (currentPositionData.occupied)
            {
                Output[index] = 0;
            }

            int2 BL_pos = PositionFromIndex(index, ProbeCounts);
            float2 directionSum = 0;
            int smallestDist = 99999;
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int yOffset = -1; yOffset <= 1; yOffset++)
                {
                    int2 newPos = BL_pos + new int2(xOffset, yOffset);
                    if (!PositionInBounds(newPos, ProbeCounts)) 
                        continue;
                    var data = TileData[IndexFromPosition(newPos, ProbeCounts)];
                    int dist = data.distance;
                    if (!data.occupied && dist <= smallestDist)
                    {
                        directionSum = new float2(xOffset, yOffset);
                        smallestDist = dist;
                    }
                }
            }

            if (math.dot(directionSum, directionSum) > 0.01)
            {
                directionSum = math.normalize(directionSum);
            }

            Output[index] = directionSum;
        }
    }
}