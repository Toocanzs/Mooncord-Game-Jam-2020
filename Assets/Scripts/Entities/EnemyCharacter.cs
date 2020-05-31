using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    private bool update_disabled;

    void Start()
    {
        // @TEMP:
        var brain = GetComponent<AIBrain>();
        brain.EnableBrain(true);
        
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
        var delay_seq = DOTween.Sequence();
        delay_seq.SetDelay(2.0f).OnComplete(() => {
            delay_seq = null;
            Destroy(gameObject);
        });
    }
}
