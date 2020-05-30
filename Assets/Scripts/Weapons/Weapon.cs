using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Weapon : MonoBehaviour
{
    public float fire_cooldown;
    public float max_ammo;
    public GameObject projectile;

    protected float fire_cooldown_remaining;
    protected float current_ammo;
    protected GameObject owner;

    void Update()
    {
        fire_cooldown_remaining = Math.Max(fire_cooldown_remaining - Time.deltaTime, 0f);
    }

    public virtual bool CanFire() {
        // other conditions for more complex weapons?
        bool ammo_avail = max_ammo > 0f ? current_ammo > 0f : true;
        return fire_cooldown_remaining == 0f && ammo_avail;
    }

    public virtual bool TryFire() {
        if (CanFire()) {
            Fire();
            return true;
        } else {
            return false;
        }
    }

    public void SetOwner(GameObject gameobject) {
        owner = gameobject;
    }
    public GameObject GetOwner() {
        return owner;
    }

    protected virtual void Fire() {
        fire_cooldown_remaining = fire_cooldown;
        if(max_ammo > 0) {
            current_ammo--;
        }
    }

    protected virtual Vector2 GetFireDirection() {
        // @TODO: controller direction if that is valid
        var mouse_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (owner) {
            var position_diff = mouse_position - owner.transform.position;
            return new Vector2(position_diff.x, position_diff.y).normalized;
        } else {
            Debug.LogWarning("Weapon has no owner set!");
            return new Vector2();
        }
    }
}
