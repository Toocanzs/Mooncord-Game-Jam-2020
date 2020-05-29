using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float fire_cooldown;
    public float max_ammo;

    protected float fire_cooldown_remaining;
    protected float current_ammo;

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

    protected virtual void Fire() {
        fire_cooldown_remaining = fire_cooldown;
        if(max_ammo > 0) {
            current_ammo--;
        }
    }
}
