﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : CharacterMovement
{
    public float dash_velocity;
    public float dash_duration;
    public float dash_cooldown;

    private Rigidbody2D rigid_body;
    private float dash_cooldown_remaining;
    private float dash_active_time_remaining;
    private Vector2 dash_direction;
    private Vector3 default_weapon_transform_position;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        var weapon_transform = transform.Find("weapon_position");
        if (!weapon_transform) {
            Debug.LogError("Unable to find weapon position transform");
        } else {
            default_weapon_transform_position = weapon_transform.localPosition;
        }

    }
    public override void StopMovement() {
        rigid_body.velocity = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (!ControlManager.IsInputEnabled()) {
            return;
        }
        UpdateMovement();
        SetWeaponTransform();
    }

    public void ChangeSpeedBy(float value) {
        move_speed = Mathf.Clamp(move_speed + value, move_speed_limits.min, move_speed_limits.max);
    }

    private void UpdateMovement() {
        var axis_horizontal = Input.GetAxis("Horizontal");
        var axis_vertical = Input.GetAxis("Vertical");
        dash_cooldown_remaining = Mathf.Max(dash_cooldown_remaining - Time.deltaTime, 0f);
        dash_active_time_remaining = Mathf.Max(dash_active_time_remaining - Time.deltaTime, 0f);
        if(dash_active_time_remaining > 0f) {
            rigid_body.velocity = dash_direction * dash_velocity;
        } else if (Input.GetKey("space") && dash_cooldown_remaining == 0f) {
            var direction = new Vector2(axis_horizontal, axis_vertical);
            dash_direction = direction.normalized;
            rigid_body.velocity = dash_direction * dash_velocity;
            dash_cooldown_remaining = dash_cooldown;
            dash_active_time_remaining = dash_duration;
        } else {
            rigid_body.velocity = new Vector2(axis_horizontal * move_speed, axis_vertical * move_speed);
        }

    }

    private void SetWeaponTransform() {
        var axis_horizontal = Input.GetAxis("Horizontal");
        var weapon_transform = transform.Find("weapon_position");
        var updated_position = default_weapon_transform_position;
        if (axis_horizontal < 0f) {
            updated_position.x = -default_weapon_transform_position.x;
        } 
        if (!weapon_transform) {
            Debug.LogError("Unable to find weapon position transform");
        } else {
            weapon_transform.localPosition = updated_position;
        }
    }
}
