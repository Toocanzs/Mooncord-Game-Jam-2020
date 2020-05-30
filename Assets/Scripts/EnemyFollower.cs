using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyFollower : MonoBehaviour
{
    public float speed = 5;
    void Start()
    {
        
    }
    
    void Update()
    {
        if (PathfindingManager.Instance != null)
        {
            float2 dir = PathfindingManager.Instance.GetDirectionToPlayer(transform.position);
            transform.position = (float3) transform.position + (new float3(dir,0) * Time.deltaTime * speed);
        }
    }
}
