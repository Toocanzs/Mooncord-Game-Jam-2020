using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class RoomManager : MonoBehaviour
{
    public bool debug_disable;
    public float room_exit_fade_out_time;
    public float room_enter_fade_in_time;
    public List<GameObject> room_prefabs = new List<GameObject>();
    private Room active_room_instance;
    private int current_room_index;
    private static RoomManager instance;

    private void Awake() {
        //DontDestroyOnLoad(this.gameObject);
        if(instance == null) {
            instance = this;
        }
    }

    void Start()
    {
        if (debug_disable) {
            return;
        }
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
        ControlManager.SetInputEnabled(false);
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(instance.room_exit_fade_out_time, 1f, () => {
            Destroy(instance.active_room_instance.gameObject);
            instance.current_room_index++;
            CreateNextRoom();
        });
    }

    private static void CreateNextRoom() {
        if(instance.room_prefabs.Count > instance.current_room_index) {
            var room_instance = Instantiate(instance.room_prefabs[instance.current_room_index]);
            var room_component = room_instance.GetComponent<Room>();
            instance.active_room_instance = room_component;
            var game_camera = GameCamera.GetCamera();
            game_camera.StartFade(instance.room_exit_fade_out_time, 0f, () => {
                ControlManager.SetInputEnabled(true);
                room_component.ActivateRoom();
            });
        }

    }
}
