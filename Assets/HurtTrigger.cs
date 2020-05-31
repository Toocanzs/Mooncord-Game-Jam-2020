using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtTrigger : MonoBehaviour
{
    public int damage = 1;

    public bool destroySelf = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(typeof(PlayerCharacter), out var character))
        {
            character.GetComponent<HealthComponent>().ChangeHealth(-damage);
            if(destroySelf)
                Destroy(gameObject);
            //TODO: Change to stay and implement iframes
        }
    }
}
