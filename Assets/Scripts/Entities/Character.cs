using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{

    protected HealthComponent health_component;
    virtual protected void Awake() {
        health_component = GetComponent<HealthComponent>();
        if (!health_component) {
            Debug.LogError("Character type requires HealthComponent!");
        }
    }

    protected virtual void OnDeath() {

    }
}
