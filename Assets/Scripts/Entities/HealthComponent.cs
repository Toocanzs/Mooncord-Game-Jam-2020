using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public int max_health;
    private int current_health;

    public int Health {
        get { return current_health; }
    }

    private void Awake() {
        current_health = max_health;
    }


    public void ChangeHealth(int value) {
        //Debug.Log("health changed by " + value);
        current_health = Mathf.Clamp(current_health + value, 0, max_health);
    }

    public bool isDead() {
        return current_health == 0;
    }
       
}
