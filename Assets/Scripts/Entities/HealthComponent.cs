using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public float max_health;
    private float current_health;

    public float Health {
        get { return current_health; }
    }

    void Start()
    {
        current_health = max_health;
    }

    public void ChangeHealth(float value) {
        current_health = Mathf.Clamp(current_health + value, 0f, max_health);
    }

    public bool isDead() {
        return current_health == 0f;
    }
       
}
