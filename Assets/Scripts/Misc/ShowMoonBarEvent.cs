using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShowMoonBarEvent : MonoBehaviour, ISequenceEvent {

    public GameObject bar_prefab;

    public void StartEvent(Action complete_callback) {
        Instantiate<GameObject>(bar_prefab);
        var fader_renderer = GetComponent<SpriteRenderer>();
        Color target_color = new Color(0f, 0f, 0f, 0f);
        DOTween.To(() => fader_renderer.color, x => fader_renderer.color = x, target_color, 2).OnComplete(() => {
            complete_callback?.Invoke();
        });
    }
}
