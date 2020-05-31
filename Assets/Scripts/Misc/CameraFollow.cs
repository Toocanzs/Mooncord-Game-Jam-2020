using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class CameraFollow : MonoBehaviour
{
    public Transform target_transform;
    public float FollowDelay;
    public float maxCursorDistance = 10;
    private Vector3 followVelocity;

    //private Vector3 finalTarget;

    //public Vector3 FollowPosition { get { return finalTarget; } }
    public void SetTarget(Transform target) {
        target_transform = target;
    }

    private void Start()
    {
        if (!target_transform)
            return;
        transform.position = target_transform.position;
    }

    private void Update()
    {
        TrackTarget();
    }


    private void TrackTarget()
    {
        if (!target_transform)
            return;
        Vector2 targetPos = new Vector2(target_transform.position.x, target_transform.position.y);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 offset = mousePos - targetPos;
        float length = offset.magnitude;
        Vector2 direction = offset.normalized;
        Vector2 clampedOffset = math.smoothstep(0, maxCursorDistance, length) * maxCursorDistance * direction;
        Vector2 target = (Vector2)target_transform.position + clampedOffset / 2.5f;

        var final_target = new Vector3(target.x, target.y, -10f);
        //var final_target = new Vector3(target.x, target.y, 0f);
        transform.position = Vector3.SmoothDamp(transform.position, final_target, ref followVelocity, FollowDelay);
    }

    public void JumpTo(Vector3 position) {
        transform.position = position;
    }
}
