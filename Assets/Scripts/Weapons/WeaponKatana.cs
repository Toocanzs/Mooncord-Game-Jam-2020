using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponKatana : Weapon
{
    public float swing_time_total;
    public float health_change_value;
    private float swing_time_current;
    private float swing_start_rotation;
    private float start_direcition;

    protected override void Fire() {
        base.Fire();
        var weapon_direction = GetFireDirection();
        var direction_angle = Mathf.Rad2Deg * Mathf.Atan2(weapon_direction.y, weapon_direction.x);
        float start_offset = 0f;
        start_direcition = -1f;
        if(direction_angle > 90f || direction_angle < -90f) {
            start_direcition = 1f;
            start_offset = -180f;
        }
        swing_start_rotation = Mathf.Rad2Deg * Mathf.Atan2(weapon_direction.y, weapon_direction.x) + start_offset;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, swing_start_rotation));
        swing_time_current = swing_time_total;
        var collider = GetComponent<Collider2D>();
        collider.enabled = true;
    }

    protected override void Update() {
        base.Update();
        swing_time_current = Mathf.Max(swing_time_current - Time.deltaTime, 0f);
        if(swing_time_current > 0f) {
            float swing_alpha = 1f - (swing_time_current / swing_time_total);
            float swing_percent = 90f * swing_alpha;
            transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, swing_start_rotation + (swing_percent * start_direcition)));
        } else {
            transform.localRotation = Quaternion.identity;
            var collider = GetComponent<Collider2D>();
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == gameObject) {
            return;
        }
        var health_component = collision.gameObject.GetComponent<HealthComponent>();
        if (health_component) {
            var team_component = collision.gameObject.GetComponent<TeamComponent>();
            if (team_component) {
                var owner_team_component = owner.GetComponent<TeamComponent>();
                if (team_component.Team != owner_team_component.Team) {
                    health_component.ChangeHealth(health_change_value);
                }
            }
        }
    }


}
