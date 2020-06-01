using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FloatRange {
    public float min;
    public float max;
}

public class CharacterMovement : MonoBehaviour
{
    protected virtual void Awake() {

    }
    public virtual void StopMovement() {

    }

    public virtual void AddPush(Vector2 velocity) {

    }

    public float move_speed;
    public FloatRange move_speed_limits;
}
