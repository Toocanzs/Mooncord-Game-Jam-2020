using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System;

public class TextElement : MonoBehaviour
{

    public bool start_hidden;
    private TMP_Text text_component;
    private bool next_pressed;
    private bool is_revealing;
    private DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions> reveal_tween;
    public event EventHandler OnComplete;

    void Awake()
    {
        text_component = GetComponentInChildren<TMP_Text>();
        if (start_hidden) {
            text_component.maxVisibleCharacters = 0;
            is_revealing = true;
        }
    }

    public void SetText(string text) {
        text_component.text = text;
    }

    public void StartHidden(bool value) {
        text_component.maxVisibleCharacters = value ? 0 : text_component.text.Length;
        is_revealing = value;
    }

    public int GetTextLength() {
        return text_component.text.Length;
    }

    public void RevealText(float time, System.Action callback = null) {
        var char_count = text_component.text.Length;
        if(time == 0f) {
            text_component.maxVisibleCharacters = char_count;
            if (callback != null) {
                callback.Invoke();
            }
            return;
        }
        reveal_tween = DOTween.To(() => text_component.maxVisibleCharacters, x => text_component.maxVisibleCharacters = x, char_count, time).OnComplete(() => {
            is_revealing = false;
            if (callback != null) {
                callback.Invoke();
            }
            reveal_tween = null;
        });
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) {
            next_pressed = true;
        }
        if(!Input.GetKey(KeyCode.Space) && next_pressed) {
            next_pressed = false;
            if (is_revealing && reveal_tween != null) {
                reveal_tween.Complete();
            } else {
                OnComplete?.Invoke(this,null);
            }
        }
    }
}
