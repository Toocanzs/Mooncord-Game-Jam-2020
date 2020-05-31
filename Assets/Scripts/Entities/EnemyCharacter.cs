using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    void Start()
    {
        // @TEMP:
        var brain = GetComponent<AIBrain>();
        brain.EnableBrain(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health_component.isDead()) {
            OnDeath();
        }
    }

    protected override void OnDeath() {
        base.OnDeath();
        Destroy(gameObject);
    }
}
