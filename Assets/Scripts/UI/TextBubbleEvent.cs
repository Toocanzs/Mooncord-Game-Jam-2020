using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBubbleEvent : MonoBehaviour, ISequenceEvent
{
    public bool attach_to_character;
    public bool attach_to_location;
    public Vector3 attach_location;
    public SpriteRenderer hint_sprite_renderer;

    public void StartEvent(Action complete_callback) {
        var bubble_component = GetComponentInChildren<TextBubble>();
        if (attach_to_character) {
            var character = PlayerCharacter.GetPlayerCharacter();
            var anchor = character.GetTextBubbleAnchor();
            bubble_component.SetAnchor(anchor);
        } else if (attach_to_location) {
            bubble_component.SetPosition(attach_location);
        }
        var text_element = GetComponentInChildren<TextElement>();
        text_element.StartHidden(true);
        var text_length = text_element.GetTextLength();
        text_element.RevealText(text_length / 15);
        text_element.OnComplete += (inst, args) => { complete_callback?.Invoke(); };

        if(hint_sprite_renderer != null) {
            var seq = DOTween.Sequence();
            seq.SetDelay(2.0f);
            seq.Append(hint_sprite_renderer.DOColor(new Color(1f, 1f, 1f, 1f), 0.4f));
            seq.OnComplete(() => seq = null);
        }
    }

    private void OnDestroy() {
        DOTween.Kill(this);
    }
}
