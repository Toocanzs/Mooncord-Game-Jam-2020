using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Pickupable, IPikcupable
{
    public int health_amount;

    public void DisplayPickupKey() {
    }

    public void DisplayPickupKey(bool value) {
        // nothing
    }

    public void OnCharacter(GameObject character_gameobject) {
        var player_health_component = character_gameobject.GetComponent<HealthComponent>();
        player_health_component.ChangeHealth(health_amount);
        DoPickup();
    }

    public void OnCharacterExit(GameObject character_gameobject) {
        // nothing
    }
}
