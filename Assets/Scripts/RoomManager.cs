using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<Room> rooms = new List<Room>();
    private static RoomManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
        // @TEMP:
        rooms = Object.FindObjectsOfType<Room>().ToList();
    }

    void Start()
    {
        if(rooms.Count == 0) {
            Debug.LogWarning("Please assign some Rooms to the RoomManager!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Room GetActiveRoom() {
        return instance.rooms.Count != 0 ? instance.rooms[0] : null;
    }

    public static void ExitCurrentRoom() {
        if(instance.rooms.Count > 0) {
            instance.rooms.RemoveAt(0);
        }
    }
}
