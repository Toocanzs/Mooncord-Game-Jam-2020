using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour
{

    public Sprite closed_sprite;
    public Sprite open_sprite;
    
    public RoomRelativeDirection exit_direction;

    private void Awake() {
        var sprite_render = GetComponent<SpriteRenderer>();
        sprite_render.sprite = closed_sprite;
    }

    public void SetExitable() {
        var exit_trigger = GetComponent<Collider2D>();
        if (exit_trigger == null) {
            Debug.LogWarning("Cannot SetExitable on RoomExit, no exit collision found!");
            return;
        }
        var sprite_render = GetComponent<SpriteRenderer>();
        sprite_render.sprite = open_sprite;
        var indicator_trans = transform.Find("indicator_arrow");
        indicator_trans.gameObject.SetActive(true);
        exit_trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var player_character = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player_character) {
            var movement = player_character.GetComponent<CharacterMovement>();
            movement.StopMovement();
            var current_room = RoomManager.GetActiveRoom();
            if (current_room && current_room.CanExit()) {
                current_room.ExitRoom(this);
            }
        }
    }
}
