using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    public GameObject temp_starting_weapon;
    private GameObject equiped_weapon;
    private List<WeaponPickup> nearby_weapon_pickups = new List<WeaponPickup>();

    private WeaponPickup pickupkey_down_item;

    public Weapon GetWeapon() {
        if (!equiped_weapon) {
            Debug.LogWarning("TryFireWeapon with no equipped weapon!");
            return null;
        }
        return equiped_weapon.GetComponent<Weapon>();
    }

    public void PickupWeapon(GameObject weapon_gameobject) {
        // @TODO; always drop weapon?
        DropWeapon();
        var weapon_instance = Instantiate<GameObject>(weapon_gameobject);
        equiped_weapon = weapon_instance;
        var weapon = weapon_instance.GetComponent<Weapon>();
        if (!weapon) {
            Debug.LogError("Trying to use PickupWeapon on GameObject with no Weapon component!");
            return;
        }
        // @TODO: move weapon to character stuff...
        weapon.SetOwner(gameObject);
        var weapon_position_transform = gameObject.transform.Find("weapon_position");
        if (!weapon_position_transform) {
            Debug.LogError("Missing weapon position child transform!");
            return;
        }
        weapon_instance.transform.parent = weapon_position_transform;
        weapon_instance.transform.localPosition = Vector3.zero;
    }

    public void DropWeapon() {
        if (equiped_weapon) {
            var weapon = equiped_weapon.GetComponent<Weapon>();
            bool has_useable_ammo = weapon.HasAmmo() ? weapon.GetCurrentAmmo() > 0 : true;
            if (weapon.is_droppable && has_useable_ammo) {
                //weapon.SetOwner(null);
                // @TODO: drop weapon set max ammo?

            } else {
            }
            // @TODO: is this right?
            Destroy(equiped_weapon);
        }
    }

    void Start()
    {
        // @TEMP startig weapon stuff
        PickupWeapon(temp_starting_weapon);
    }

    public void AddNearbyWeaponPickup(WeaponPickup nearby_pickup) {
        nearby_weapon_pickups.Add(nearby_pickup);
    }

    public void RemoveNearbyWeaponPickup(WeaponPickup nearby_pickup) {
        nearby_weapon_pickups.Remove(nearby_pickup);
        if(pickupkey_down_item == nearby_pickup) {
            pickupkey_down_item = null;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!ControlManager.IsInputEnabled()) {
            return;
        }
        if (Input.GetButton("Fire1")) {
            TryFireWeapon();
        }
        List<Tuple<float, WeaponPickup>> nearby_weapons_distnaces = new List<Tuple<float, WeaponPickup>>();
        nearby_weapon_pickups.ForEach((WeaponPickup pickup) => {
            var pickup_distance = Vector3.Distance(gameObject.transform.position, pickup.gameObject.transform.position);
            nearby_weapons_distnaces.Add(new Tuple<float, WeaponPickup>(pickup_distance, pickup));
        });
        nearby_weapons_distnaces.Sort((x,y)=>x.Item1.CompareTo(y.Item1));
        for(int i = 0; i < nearby_weapons_distnaces.Count; i++) {
            if(i == 0) {
                nearby_weapons_distnaces[i].Item2.DisplayPickupKey(true);
            } else {
                nearby_weapons_distnaces[i].Item2.DisplayPickupKey(false);
            }
        }
        if (Input.GetButton("TryPickup")) {
            if(nearby_weapons_distnaces.Count > 0) {
                pickupkey_down_item = nearby_weapons_distnaces[0].Item2;
            }
        } else if(Input.GetButtonUp("TryPickup") && pickupkey_down_item) {
            if(nearby_weapons_distnaces.Count > 0) {
                if(pickupkey_down_item == nearby_weapons_distnaces[0].Item2) {
                    var weapon_pickup = nearby_weapons_distnaces[0].Item2;
                    var weapon_prefab = weapon_pickup.GetWeaponTypePrefab();
                    PickupWeapon(weapon_prefab);
                    weapon_pickup.DoPickup();
                    pickupkey_down_item = null;
                }
            }
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
