using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    void Start()
    {
        
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
