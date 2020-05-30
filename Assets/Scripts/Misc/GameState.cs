using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public bool debug_disable;

    void Start()
    {
        if (debug_disable) {
            return;
        }
        ControlManager.SetInputEnabled(false);
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(0f, 1f);
        game_camera.StartFade(1.5f, 0f, () => {
            //var control_manager = GetComponent<ControlManager>();
            ControlManager.SetInputEnabled(true);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
