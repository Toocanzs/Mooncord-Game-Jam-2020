using System;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermBrain : AIBrain
{

    public float spit_cooldown;
    public float minimum_seek_distance;
    public float spit_delay_between_projectiles;

    protected int spit_projectiles_remain;
    protected float spit_between_remain;
    protected float spit_cooldown_remain;

    protected override void Update() {
        //base.Update();
        var player_distance = GetDistanceToPlayer();
        if (!brain_enabled) {
            var health_component = GetComponent<HealthComponent>();
            if(enable_on_distance_or_damage && !health_component.isDead()) {
                if(player_distance <= enable_distance) {
                    brain_enabled = true;
                }
            }
            return;
        }
        if (seek_time_remain > 0f && player_distance > minimum_seek_distance) {
            var seek_direction = PathfindingManager.Instance.GetDirectionToPlayer(transform.position);
            enemy_movement.MoveDirection(seek_direction);
            seek_time_remain = Mathf.Max(0f, seek_time_remain - Time.deltaTime);
        } else {
            enemy_movement.MoveDirection(new float2(0f,0f));
            if(paused_seek_time_remain > 0f) {
                paused_seek_time_remain = Mathf.Max(0f, paused_seek_time_remain - Time.deltaTime);
            } else {
                SetSeekTime();
            }
        }
        if(spit_cooldown_remain > 0f) {
            spit_cooldown_remain = Mathf.Max(0f, spit_cooldown_remain - Time.deltaTime);
        } else {
            if(spit_projectiles_remain == 0) {
                // randomize spit projectiles
                spit_projectiles_remain = UnityEngine.Random.Range(1, 4);
                spit_between_remain = spit_delay_between_projectiles;
            } else {
                if(spit_between_remain > 0f) {
                    spit_between_remain = Mathf.Max(0f, spit_between_remain - Time.deltaTime);
                } else {
                    Debug.Log("Spit!");
                    // @TODO: do sppit
                    spit_projectiles_remain--;
                    if(spit_projectiles_remain == 0) {
                        spit_cooldown_remain = UnityEngine.Random.Range(spit_cooldown, spit_cooldown + 2f);
                    }
                }
            }
        }
    }

    protected virtual void TrySpit() {

    }
}
