using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameStartupSequence : MonoBehaviour
{

    public List<GameObject> sequence_items = new List<GameObject>();
    private int sequence_index;
    private GameObject active_sequence_item;
    private bool sequence_active;
    private void Awake() {
        sequence_active = true;
    }

    public bool IsSequenceActive() {
        return sequence_active;
    }

    public void StartGameIntroSequence() {
        ControlManager.SetInputEnabled(false);
        // spawn room..
        var room_setup_properties = new RoomSetupProperties();
        room_setup_properties.enter_from = RoomRelativeDirection.NORTH;
        RoomManager.CreateNextRoom(room_setup_properties);
        // camera stuff..
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(0f, 1f);
        var follow_component = game_camera.gameObject.GetComponent<CameraFollow>();
        var player = PlayerCharacter.GetPlayerCharacter();
        follow_component.JumpTo(player.gameObject.transform.position);
        follow_component.SetTarget(player.gameObject.transform);
        // start sequence...
        NextSequenceItem();
    }

    private void NextSequenceItem() {
        if(sequence_index < sequence_items.Count) {
            if (active_sequence_item) {
                Destroy(active_sequence_item);
            }
            active_sequence_item = Instantiate<GameObject>(sequence_items[sequence_index]);
            var sequence_event = active_sequence_item.GetComponent<ISequenceEvent>();
            sequence_event.StartEvent(NextSequenceItem);
            sequence_index++;
        } else {
            sequence_active = false;
            EndGameIntroSequence();
        }
    }

    public void EndGameIntroSequence() {
        if (active_sequence_item) {
            Destroy(active_sequence_item);
        }
        ControlManager.SetInputEnabled(true);
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(0f, 0f);
        var game_cam = GameCamera.GetCamera();
        var player_char = PlayerCharacter.GetPlayerCharacter();
        var follow_comp = game_cam.GetComponent<CameraFollow>();
        follow_comp.SetTarget(player_char.gameObject.transform);

        // activate the room after 1s delay
        var seq = DOTween.Sequence();
        seq.SetDelay(1.0f).OnComplete(() => {
            var active_room = RoomManager.GetActiveRoom();
            active_room.ActivateRoom();
            seq = null;
            var hints = FindObjectsOfType<HintUIElement>().ToList();
            hints.ForEach((hint) => {
                if (hint.hint_type == HintType.SPRINT) {
                    hint.Show(1.2f);
                }
            });
        });

    }

    public void SkipSequence() {

    }
}
