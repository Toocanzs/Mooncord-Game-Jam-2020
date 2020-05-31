using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeCameraEvent : MonoBehaviour, ISequenceEvent
{
    public float fade_in_time;
    public void StartEvent(Action complete_callback) {
        var game_cam = GameCamera.GetCamera();
        game_cam.StartFade(fade_in_time, 0f, () => complete_callback?.Invoke());
    }

}
