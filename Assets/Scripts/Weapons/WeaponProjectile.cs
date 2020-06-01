using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Tilemaps;

public struct WeaponProjectileProperties {
    public Vector2 direction;
    public Team team;
}

public abstract class WeaponProjectile : MonoBehaviour
{

    private Collider2D trigger;
    private Rigidbody2D rigid_body;

    private Team team;

    public bool can_friendly_fire;
    public int health_change_on_impact;
    public float speed_initial;
    //public Vector2 velocity_change_amount;

    void Awake()
    {
        trigger = GetComponent<Collider2D>();
        rigid_body = GetComponent<Rigidbody2D>();
        if (!rigid_body) {
            Debug.LogWarning("Missing Rigidbody2D on projectile!");
            return;
        }
    }

    private void FixedUpdate() {
        //if(velocity_change_amount.x > 0f || velocity_change_amount.y > 0f) {
        //    rigid_body.velocity += velocity_change_amount;
        //}
    }

    public virtual void SetProperties(WeaponProjectileProperties properties) {
        var projectile_rotation = Mathf.Rad2Deg * Mathf.Atan2(properties.direction.y, properties.direction.x);
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, projectile_rotation));
        rigid_body.velocity = properties.direction * speed_initial;
        team = properties.team;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        var other_health_component = collision.gameObject.GetComponent<HealthComponent>();
        if (other_health_component) {
            if (can_friendly_fire) {
                other_health_component.ChangeHealth(health_change_on_impact);
                OnDeath();
            } else {
                var team_component = collision.gameObject.GetComponent<TeamComponent>();
                if(team_component.Team != team) {
                    other_health_component.ChangeHealth(health_change_on_impact);
                    OnDeath();
                }
            }
        } else {
            var rigidbody_comp = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rigidbody_comp) {
                if(rigidbody_comp.bodyType == RigidbodyType2D.Static) {
                    OnDeath();
                }
            }
            if(collision is TilemapCollider2D) {
                OnDeath();
            }
        }
    }

    protected virtual void OnDeath() {
        // @TODO: do whatever here...
        // spawn impact
        Destroy(this.gameObject);
    }

}
