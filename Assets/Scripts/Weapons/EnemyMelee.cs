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
    private Animator animator;
    private Transform last_attacked_transform; // eh, kinda jank
    private AIBrain brain;
    //private bool is_enabled;

    private void Awake() {
        brain = transform.parent.gameObject.GetComponent<AIBrain>();
        if (!brain) {
            Debug.LogWarning("Enemey Melee can't find brain!");
        }
        //is_enabled = true;
        melee_collider = GetComponentInChildren<Collider2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        
    }

    //public void SetAttackEnabled(bool value) {
    //    is_enabled = value;
    //}

    private void Update() {
        //if (!is_enabled)
        //    return;
        if(!brain || !brain.IsBrainEnabled()) {
            return;
        }

        attack_cooldown_remain = Mathf.Max(0f, attack_cooldown_remain - Time.deltaTime);
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
                last_attacked_transform = player_object.transform;
                animator.SetTrigger("bite");
                var health_component = player_object.GetComponent<HealthComponent>();
                health_component.ChangeHealth(attack_health_change);
                attack_cooldown_remain = attack_cooldown;
            }
        }
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("bite_anim_01") && last_attacked_transform){
            var bite_sprites = transform.Find("bite_sprites");
            bite_sprites.transform.position = last_attacked_transform.position + (transform.position - last_attacked_transform.position) / 2f;
        }
    }

    public bool CanAttack() {
        return attack_cooldown_remain == 0;
    }


}
