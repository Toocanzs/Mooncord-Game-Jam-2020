using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : Pickupable, IPikcupable
{
    public GameObject weapon_type_prefab;
    // @TODO:
    private int ammo_override = -1;

    public GameObject GetWeaponTypePrefab() {
        return weapon_type_prefab;
    }

    public void DisplayPickupKey(bool value) {
        var pickup_key_comp = GetComponentInChildren<PickupKeyDisplay>(true);
        if (pickup_key_comp) {
            pickup_key_comp.gameObject.SetActive(value);
        }
    }

    public override void DoPickup() {
        DisplayPickupKey(false);
        base.DoPickup();
    }

    public void OnCharacter(GameObject character_gameobject) {
        var weapon_component = character_gameobject.GetComponent<WeaponComponent>();
        weapon_component.AddNearbyWeaponPickup(this);
    }

    public void OnCharacterExit(GameObject character_gameobject) {
        var weapon_component = character_gameobject.GetComponent<WeaponComponent>();
        weapon_component.RemoveNearbyWeaponPickup(this);
        DisplayPickupKey(false);
    }
}
