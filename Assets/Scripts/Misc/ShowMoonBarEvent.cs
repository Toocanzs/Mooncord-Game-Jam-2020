using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShowMoonBarEvent : MonoBehaviour, ISequenceEvent {


    public void StartEvent(Action complete_callback) {
        var head_ui = FindObjectOfType<MoonHeadUI>();
        head_ui.FadeIn(1.0f, complete_callback);
    }
}
