using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    public GameObject temp_starting_weapon;
    private GameObject equiped_weapon;

    public void PickupWeapon(GameObject weapon_gameobject) {
        var weapon = weapon_gameobject.GetComponent<Weapon>();
        if (!weapon) {
            Debug.LogError("Trying to use PickupWeapon on GameObject with no Weapon component!");
            return;
        }
        // @TODO; always drop weapon?
        DropWeapon();
        // @TODO: move weapon to character stuff...
        weapon.SetOwner(gameObject);
        equiped_weapon = weapon_gameobject;
    }

    public void DropWeapon() {
        if (equiped_weapon) {
            var weapon = equiped_weapon.GetComponent<Weapon>();
            weapon.SetOwner(null);
        }
        // @TODO drop weapon
    }

    void Start()
    {
        // @TEMP startig weapon stuff
        PickupWeapon(Instantiate<GameObject>(temp_starting_weapon));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1")) {
            TryFireWeapon();
        }
        
    }

    public virtual void TryFireWeapon() {
        if (!equiped_weapon) {
            Debug.LogWarning("TryFireWeapon with no equipped weapon!");
            return;
        }
        var weapon_component = equiped_weapon.GetComponent<Weapon>();
        if (weapon_component) {
            weapon_component.TryFire();
        }
    }
}
