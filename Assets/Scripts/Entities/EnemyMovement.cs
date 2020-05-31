using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed;

    private Rigidbody2D rigid_body;

    private void Awake() {
        rigid_body = GetComponent<Rigidbody2D>();
    }

    public void MoveDirection(Unity.Mathematics.float2 direction) {
        rigid_body.velocity = direction * speed;
    }

}
