using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public List<GameObject> room_prefabs = new List<GameObject>();
    private Room active_room_instance;
    private int current_room_index;
    private static RoomManager instance;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        if(instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        if(room_prefabs.Count == 0) {
            Debug.LogWarning("Please assign some Rooms to the RoomManager!");
            return;
        }
        // @TEMP for now, just spawn the first room
        CreateNextRoom();
    }

    public static Room GetActiveRoom() {
        return instance.active_room_instance;
    }

    public static void ExitCurrentRoom() {
        Destroy(instance.active_room_instance.gameObject);
        instance.current_room_index++;
        CreateNextRoom();
    }

    private static void CreateNextRoom() {
        if(instance.room_prefabs.Count > instance.current_room_index) {
            var room_instance = Instantiate(instance.room_prefabs[instance.current_room_index]);
            var room_component = room_instance.GetComponent<Room>();
            room_component.ActivateRoom();
            instance.active_room_instance = room_component;
        }

    }
}
