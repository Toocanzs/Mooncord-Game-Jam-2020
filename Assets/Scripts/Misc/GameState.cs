using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool debug_disable;

    void Start()
    {
        if (debug_disable) {
            var game_camera = GameCamera.GetCamera();
            var cam_follow = game_camera.GetComponent<CameraFollow>();
            var player = PlayerCharacter.GetPlayerCharacter();
            if (!player) {
                Debug.LogError("player invalid");
            }
            cam_follow.SetTarget(player.gameObject.transform);
            return;
        }
        var startup_sequence = GetComponent<GameStartupSequence>();
        if (!startup_sequence) {
            Debug.LogError("Cant find startup sequence component");
            return;
        }
        startup_sequence.StartGameIntroSequence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
