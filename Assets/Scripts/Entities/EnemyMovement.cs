using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMovement
{

    private Rigidbody2D rigid_body;

    protected override void Awake() {
        base.Awake();
        rigid_body = GetComponent<Rigidbody2D>();
    }

    public void MoveDirection(Unity.Mathematics.float2 direction) {
        rigid_body.velocity = direction * move_speed;
    }

}
