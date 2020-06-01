using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct ItemPickupHints {

}
public class GameState : MonoBehaviour
{
    public bool debug_disable;

    private bool item_grease_hint;

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

    public void OnItemPickup(HintType hint_type) {
        switch (hint_type) {
            case HintType.ITEM_GREASE: {
                if (!item_grease_hint) {
                    item_grease_hint = true;
                    var game_ui = FindObjectOfType<GameUI>();
                    var hint_components = game_ui.gameObject.GetComponentsInChildren<HintUIElement>().ToList();
                    var grease_hint = hint_components.Find((x) => x.hint_type == HintType.ITEM_GREASE);
                    if (grease_hint) {
                        grease_hint.Show(2.0f);
                    }
                }
                break;
            }
            default:
                break;
        }
    }

}
