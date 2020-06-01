using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private bool update_disabled;
    public bool can_drop_health = true;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (update_disabled)
            return;
        if (health_component.isDead()) {
            OnDeath();
        }
    }

    protected override void OnDeath() {
        base.OnDeath();
        update_disabled = true;
        var brain = GetComponent<AIBrain>();
        if (!brain) {
            Debug.LogError("unable to find ai brain ondeath");
            return;
        }
        brain.EnableBrain(false);
        var movement = GetComponent<CharacterMovement>();
        movement.StopMovement();
        TryDropItem();
        var delay_seq = DOTween.Sequence();
        delay_seq.SetDelay(2.0f).OnComplete(() => {
            delay_seq = null;
            Destroy(gameObject);
        });
    }

    protected void TryDropItem() {
        var item_dropper = GetComponent<ItemDropper>();
        if (item_dropper && can_drop_health) {
            var character = PlayerCharacter.GetPlayerCharacter();
            var health_component = character.GetComponent<HealthComponent>();
            var health_percent = (float)health_component.Health / (float)health_component.max_health;
            var random_val = Random.Range(0f, 1f);
            var health_chance_result = random_val - health_percent - .15f;
            //Debug.Log("healthr random result: " + health_chance_result + " " + random_val + " " + health_percent);
            if(health_chance_result > 0f) {
                item_dropper.DropHealth();
            }
        }
        if (item_dropper) {
            item_dropper.DropItem();
        }
    }
}
