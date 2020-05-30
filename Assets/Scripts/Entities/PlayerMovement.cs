using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float move_speed;
    public float dash_velocity;
    public float dash_duration;
    public float dash_cooldown;

    private SpriteRenderer character_sprite;
    private Rigidbody2D rigid_body;
    private float dash_cooldown_remaining;
    private float dash_active_time_remaining;
    private Vector2 dash_direction;
    private Vector3 default_weapon_transform_position;

    void Start()
    {
        rigid_body = GetComponent<Rigidbody2D>();
        character_sprite = GetComponent<SpriteRenderer>();
        var weapon_transform = transform.Find("weapon_position");
        if (!weapon_transform) {
            Debug.LogError("Unable to find weapon position transform");
        } else {
            default_weapon_transform_position = weapon_transform.localPosition;
        }

    }

    void FixedUpdate()
    {
        UpdateMovement();
        SetSpriteFacing();
        SetWeaponTransform();
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

    private void SetSpriteFacing() {
        var axis_horizontal = Input.GetAxis("Horizontal");
        var axis_vertical = Input.GetAxis("Vertical");
        if(axis_horizontal > 0f) {
            character_sprite.flipX = false;
        } else if(axis_horizontal < 0f) {
            character_sprite.flipX = true;
        }
        if(axis_vertical > 0f) {

        }
    }
    private void SetWeaponTransform() {
        var weapon_transform = transform.Find("weapon_position");
        var flipped_position = default_weapon_transform_position;
        if (character_sprite.flipX) {
            flipped_position.x = -flipped_position.x;
        }
        if (!weapon_transform) {
            Debug.LogError("Unable to find weapon position transform");
        } else {
            weapon_transform.localPosition = flipped_position;
        }
    }
}
