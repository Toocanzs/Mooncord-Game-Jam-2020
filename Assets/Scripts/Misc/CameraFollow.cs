using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target_transform;
    public float FollowDelay;

    private Vector3 followVelocity;
    //private Vector3 finalTarget;

    //public Vector3 FollowPosition { get { return finalTarget; } }

    public void SetTarget(Transform target) {
        target_transform = target;
    } 

    private void Update()
    {
        TrackTarget();
    }

    private void TrackTarget()
    {
        Vector2 targetPos = new Vector2(target_transform.position.x, target_transform.position.y);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 offset = mousePos - targetPos;
        Vector2 direction = offset.normalized;

        Vector2 target = (Vector2)target_transform.position + (offset / 2.5f);

        var final_target = new Vector3(target.x, target.y, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, final_target, ref followVelocity, FollowDelay);
    }

    public void JumpTo(Vector3 position) {
        transform.position = position;
    }
}
