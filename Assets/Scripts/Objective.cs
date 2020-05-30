using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    private void Start() {
        var active_room = RoomManager.GetActiveRoom();
        if (!active_room) {
            Debug.LogWarning("Active room retuned null for Objective!");
            return;
        }
        active_room.RegisterObjective(this);
    }

    private void OnDestroy() {
        var active_room = RoomManager.GetActiveRoom();
        if (!active_room) {
            Debug.LogWarning("Active room retuned null for Objective!");
            return;
        }
        active_room.RemoveObjective(this);
    }
}
