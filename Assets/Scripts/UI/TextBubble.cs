using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    private Transform anchor_transform;

    public void SetAnchor(Transform transform) {
        anchor_transform = transform;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    private void Update() {
        if (anchor_transform) {
            transform.position = anchor_transform.position;
        }
    }
}
