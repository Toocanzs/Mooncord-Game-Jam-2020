using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public bool is_droppable;
    public float fire_cooldown;
    public FloatRange fire_cooldown_limits;
    public int max_ammo;
    public GameObject projectile;

    protected float fire_cooldown_remaining;
    protected int current_ammo;
    protected GameObject owner;

    public virtual void ChangeFireCooldownBy(float value) {
        fire_cooldown = Mathf.Clamp(fire_cooldown + value, fire_cooldown_limits.min, fire_cooldown_limits.max);
    }

    protected virtual void Update()
    {
        fire_cooldown_remaining = Math.Max(fire_cooldown_remaining - Time.deltaTime, 0f);
    }
    
    public bool HasAmmo() {
        return max_ammo > 0;
    }

    public int GetCurrentAmmo() {
        return current_ammo;
    }

    public virtual bool CanFire() {
        // other conditions for more complex weapons?
        bool ammo_avail = max_ammo > 0 ? current_ammo > 0 : true;
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
