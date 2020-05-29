using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetActive() {

    }

    public bool CanExit() {
        return false;
    }

    public void ExitRoom() {
        // @OTOD: whatever here...
        RoomManager.ExitCurrentRoom();

    }
}
