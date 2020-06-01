using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreasePickup : Pickupable, IPikcupable {

    public float move_speed_change_amount;
    public float fire_cooldown_change_amount;

    public void DisplayPickupKey(bool value) {
        //nothing
    }

    public void OnCharacter(GameObject character_gameobject) {
        var weapon_component = character_gameobject.GetComponent<WeaponComponent>();
        var movement = character_gameobject.GetComponent<PlayerMovement>();
        var weapon = weapon_component.GetWeapon();
        if (weapon) {
            weapon.ChangeFireCooldownBy(fire_cooldown_change_amount);
        } else {
            Debug.LogWarning("missing weapon");
        }
        if (movement) {
            movement.ChangeSpeedBy(move_speed_change_amount);
        }
        var game_state = FindObjectOfType<GameState>();
        game_state.OnItemPickup(HintType.ITEM_GREASE);
        DoPickup();
    }

    public void OnCharacterExit(GameObject character_gameobject) {
        // nothing
    }
}
