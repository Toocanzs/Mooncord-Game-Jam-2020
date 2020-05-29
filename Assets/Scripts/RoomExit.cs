using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomExit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
