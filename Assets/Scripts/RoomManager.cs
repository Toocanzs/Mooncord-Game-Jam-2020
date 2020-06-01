using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public bool debug_disable;
    public float room_exit_fade_out_time;
    public float room_enter_fade_in_time;
    public List<GameObject> room_prefabs = new List<GameObject>();

    public string bossRoomSceneName;
    private Room active_room_instance;
    private int current_room_index;
    private static RoomManager instance;

    private void Awake() {
        //DontDestroyOnLoad(this.gameObject);
        if(instance == null) {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        instance = null;
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
    }

    public static Room GetActiveRoom() {
        if (instance != null)
        {
            return instance.active_room_instance;
        }

        return null;
    }

    public static void ExitCurrentRoom(Room exited_room, RoomExit exit) {
        ControlManager.SetInputEnabled(false);
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(instance.room_exit_fade_out_time, 1f, () => {
            Destroy(instance.active_room_instance.gameObject);
            instance.current_room_index++;
            RoomSetupProperties room_setup_properties = new RoomSetupProperties();
            room_setup_properties.enter_from = exit.exit_direction;
            CreateNextRoom(room_setup_properties);
        });
    }

    public static void CreateNextRoom(RoomSetupProperties properties) {
        if(instance.room_prefabs.Count > instance.current_room_index) {
            var room_instance = Instantiate(instance.room_prefabs[instance.current_room_index]);
            var room_component = room_instance.GetComponent<Room>();
            room_component.SetupRoom(properties);
            instance.active_room_instance = room_component;

            var game_camera = GameCamera.GetCamera();

            // move camera to player's new location
            var camera_follow = game_camera.gameObject.GetComponent<CameraFollow>();
            var player = PlayerCharacter.GetPlayerCharacter();
            camera_follow.JumpTo(player.gameObject.transform.position);

            var sequence_manager = instance.gameObject.GetComponent<GameStartupSequence>();
            if (!sequence_manager) {
                Debug.LogError("room manager couldnt find startup sequence manager!");
                return;
            }
            // check if the startup sequence is playing... if so don't fade in....
            if (!sequence_manager.IsSequenceActive()) {
                // start fade in
                game_camera.StartFade(instance.room_exit_fade_out_time, 0f, () => {
                    // @ kinda jank but whatcha gonna do...
                    ControlManager.SetInputEnabled(true);
                    room_component.ActivateRoom();
                });
            }
        } else {
            // @BOSS ROOM...
            if (instance.bossRoomSceneName == null) {
                Debug.LogWarning("No boss room prefab set!");
                return;
            }
            
            DOTween.Clear();
            SceneManager.LoadScene(instance.bossRoomSceneName, LoadSceneMode.Single);
            var game_camera = GameCamera.GetCamera();
            // @Change first param to whatever fade-in-time you want
            game_camera.StartFade(instance.room_exit_fade_out_time, 0f, () => {
            });
        }

    }
}
