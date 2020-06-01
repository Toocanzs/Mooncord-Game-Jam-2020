using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AIBrain : MonoBehaviour
{
    public float enable_distance;

    private float seek_time_remain;
    private float paused_seek_time_remain;
    private EnemyMovement enemy_movement;
    private bool brain_enabled;
    private bool enable_on_distance_or_damage;

    private void Awake() {
        enemy_movement = GetComponent<EnemyMovement>();
    }

    public void EnableBrain(bool value) {
        brain_enabled = value;
        if (value) {
            SetSeekTime();
        } else {
            seek_time_remain = 0;
        }
    }

    public bool IsEnabledOnDistance() {
        return enable_on_distance_or_damage;
    }

    public void SetEnableOnDistanceOrDamage() {
        enable_on_distance_or_damage = true;
        var health_component = GetComponent<HealthComponent>();
        health_component.on_health_change += OnHealth;
    }

    public void OnHealth(int difference) {
        if(difference < 0f && !IsBrainEnabled()) {
            EnableBrain(true);
        }
    }

    public bool IsBrainEnabled() {
        return brain_enabled;
    }

    private void SetSeekTime() {
        seek_time_remain = UnityEngine.Random.Range(2f, 5f);
        paused_seek_time_remain = UnityEngine.Random.Range(1f, 5f);
    }

    private float GetDistanceToPlayer() {
        var player = PlayerCharacter.GetPlayerCharacter();
        return Vector3.Distance(player.gameObject.transform.position, transform.position);
    }

    private void Update() {

        if (!brain_enabled) {
            var health_component = GetComponent<HealthComponent>();
            if(enable_on_distance_or_damage && !health_component.isDead()) {
                var player_distance = GetDistanceToPlayer();
                if(player_distance <= enable_distance) {
                    brain_enabled = true;
                }
            }
            return;
        }
        if (seek_time_remain > 0f) {
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
    }
}
