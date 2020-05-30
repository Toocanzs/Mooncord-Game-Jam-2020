﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour
{

    public void SetExitable() {
        var exit_trigger = GetComponent<Collider2D>();
        if (exit_trigger == null) {
            Debug.LogWarning("Cannot SetExitable on RoomExit, no exit collision found!");
            return;
        }
        var animator = GetComponent<Animator>();
        if (!animator) {
            Debug.LogWarning("Missing animator component on RoomExit!");
            return;
        } else {
            animator.SetTrigger("open");
        }
        exit_trigger.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        var player_character = collision.gameObject.GetComponent<PlayerCharacter>();
        if (player_character) {
            var current_room = RoomManager.GetActiveRoom();
            if (current_room && current_room.CanExit()) {
                current_room.ExitRoom();
            }
        }
    }
}
