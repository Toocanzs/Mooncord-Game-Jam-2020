using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class GameCamera : MonoBehaviour { 

    private SpriteRenderer fade_sprite_renderer;

    private static GameCamera instance;

    private void Awake() {
        if (!instance) {
            instance = this;
        }
        var fade_transform = transform.Find("fade");
        if (!fade_transform) {
            Debug.LogError("Unable to find fade transform on game camera");
            return;
        }
        fade_sprite_renderer = fade_transform.GetComponent<SpriteRenderer>();
        if (!fade_sprite_renderer) {
            Debug.LogError("Fade transform missing sprite renderer!");
        }
    }

    public void StartFade(float time, float alpha,  Action callback = null) {
        fade_sprite_renderer.enabled = true;
        Color target_color = new Color(0f, 0f, 0f, alpha);
        DOTween.To(() => fade_sprite_renderer.color, x =>  fade_sprite_renderer.color = x, target_color, time).OnComplete(() => {
            if(alpha == 0f) {
                fade_sprite_renderer.enabled = false;
            }
            if (callback != null) {
                callback.Invoke();
            }
        });
    }

    public static GameCamera GetCamera() {
        return instance;
    }

}
