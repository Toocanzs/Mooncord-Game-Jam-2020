using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    public bool destroy_owner_on_pickup = true;
    private Collider2D trigger;

    private void Start() {
        trigger = GetComponent<Collider2D>();
    }

    public virtual void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<PlayerCharacter>()) {
            var pickup_interface = GetComponent<IPikcupable>();
            if(pickup_interface != null) {
                pickup_interface.OnCharacter(other.gameObject);
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other) {
        if (other.GetComponent<PlayerCharacter>()) {
            var pickup_interface = GetComponent<IPikcupable>();
            if(pickup_interface != null) {
                pickup_interface.OnCharacterExit(other.gameObject);
            }
        }
    }

    public virtual void DoPickup() {
        if (destroy_owner_on_pickup) {
            Destroy(gameObject);
        }
    }


}

public interface IPikcupable {
    void OnCharacter(GameObject character_gameobject);
    void OnCharacterExit(GameObject character_gameobject);
    void DisplayPickupKey(bool value);
}
