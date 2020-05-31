using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMovement
{

    public float push_dampening;

    private Rigidbody2D rigid_body;
    private float push_time_remaining;
    private Vector2 push_velocity;

    protected override void Awake() {
        base.Awake();
        rigid_body = GetComponent<Rigidbody2D>();
    }

    public void MoveDirection(Unity.Mathematics.float2 direction) {
        if(push_time_remaining > 0f) {
            push_time_remaining = Mathf.Max(0f, push_time_remaining - Time.deltaTime);
            rigid_body.velocity = push_velocity;
        } else {
            rigid_body.velocity = direction * move_speed;
        }
    }

    public override void AddPush(Vector2 velocity) {
        base.AddPush(velocity);
        push_time_remaining = 0.2f;
        push_velocity = velocity;
        DOTween.To(() => push_velocity, x => push_velocity = x, Vector2.zero, push_time_remaining);
    }

}
