using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartupSequence : MonoBehaviour
{

    public List<GameObject> sequence_items = new List<GameObject>();
    private int sequence_index;
    private GameObject active_sequence_item;

    public void StartGameIntroSequence() {
        ControlManager.SetInputEnabled(false);
        var game_camera = GameCamera.GetCamera();
        game_camera.StartFade(0f, 1f);
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
    }
}
