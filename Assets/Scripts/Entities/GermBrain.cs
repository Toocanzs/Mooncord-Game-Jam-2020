using System;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct IntRange {
    public int min;
    public int max;
}

public class GermBrain : AIBrain
{

    public GameObject germ_projectile;
    public float spit_cooldown;
    public float minimum_seek_distance;
    public float spit_delay_between_projectiles;
    public IntRange projectiles_per_volley;

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
                spit_projectiles_remain = UnityEngine.Random.Range(projectiles_per_volley.min,projectiles_per_volley.max);
                spit_between_remain = spit_delay_between_projectiles;
            } else {
                if(spit_between_remain > 0f) {
                    spit_between_remain = Mathf.Max(0f, spit_between_remain - Time.deltaTime);
                } else {
                    Debug.Log("Spit!");
                    TrySpit();
                    // @TODO: do sppit
                    spit_projectiles_remain--;
                    spit_between_remain = UnityEngine.Random.Range(spit_delay_between_projectiles, spit_delay_between_projectiles + 0.5f);
                    if(spit_projectiles_remain == 0) {
                        spit_cooldown_remain = UnityEngine.Random.Range(spit_cooldown, spit_cooldown + 2f);
                    }
                }
            }
        }
    }

    protected virtual void TrySpit() {

        var projectile_instance = Instantiate<GameObject>(germ_projectile, transform);
        var projectile_component = projectile_instance.GetComponent<WeaponProjectile>();
        if (!projectile_component) {
            Debug.LogError("No WeaponProjectile component found on instantiated projectile!");
            return;
        }
        WeaponProjectileProperties projectile_properties;
        var fire_direction = GetSpitDirection();
        projectile_properties.direction = fire_direction;
        var team = GetComponent<TeamComponent>();
        projectile_properties.team = team.Team;
        projectile_component.SetProperties(projectile_properties);
    }

    protected Vector2 GetSpitDirection() {
        var character = PlayerCharacter.GetPlayerCharacter();
        var x_variance = UnityEngine.Random.Range(-0.2f, 0.2f);
        var y_variance = UnityEngine.Random.Range(-0.2f, 0.2f);
        var char_p = character.transform.position;
        var target_position = new Vector3(char_p.x + x_variance, char_p.y + y_variance, 0f);
        var diff = target_position - transform.position;
        diff.Normalize();
        return new Vector2(diff.x, diff.y);
    }
}
