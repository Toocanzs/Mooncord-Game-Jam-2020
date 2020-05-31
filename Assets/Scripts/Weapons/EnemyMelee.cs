using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public int attack_health_change;
    public float attack_cooldown;

    private float attack_cooldown_remain;
    private Collider2D melee_collider;

    private void Awake() {
        melee_collider = GetComponentInChildren<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        
    }

    private void Update() {
        if (CanAttack()) {
            ContactFilter2D filter = new ContactFilter2D();
            List<Collider2D> results = new List<Collider2D>();
            melee_collider.OverlapCollider(filter, results);
            GameObject player_object = null;
            results.ForEach((other) => {
                var player_compnent = other.gameObject.GetComponent<PlayerCharacter>();
                if (player_compnent) {
                    player_object = player_compnent.gameObject;
                }
            });
            if (player_object != null) {
                var health_component = player_object.GetComponent<HealthComponent>();
                health_component.ChangeHealth(attack_health_change);
                Debug.Log("ATTACKING");
            }
        }
    }

    public bool CanAttack() {
        return attack_cooldown_remain == 0;
    }


}
