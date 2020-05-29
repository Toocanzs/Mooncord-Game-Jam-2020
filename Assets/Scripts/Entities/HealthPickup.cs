using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IPikcupable
{
    public float health_amount;

    public void OnCharacter(GameObject character_gameobject) {
        var player_health_component = character_gameobject.GetComponent<HealthComponent>();
        player_health_component.ChangeHealth(health_amount);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
