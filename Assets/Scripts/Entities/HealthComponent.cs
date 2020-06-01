using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public delegate void OnHealthChange(int difference);
    public OnHealthChange on_health_change;

    public int max_health;
    private int current_health;

    public int Health {
        get { return current_health; }
    }

    private void Awake() {
        current_health = max_health;
    }


    public void ChangeHealth(int value) {
        var last_health = current_health;
        current_health = Mathf.Clamp(current_health + value, 0, max_health);
        var health_diff = current_health - last_health;
        on_health_change?.Invoke(health_diff);
    }

    public bool isDead() {
        return current_health == 0;
    }
       
}
