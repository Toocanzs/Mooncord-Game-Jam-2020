using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using DG.Tweening.Core.Easing;

public class YouDied : MonoBehaviour
{
    public float show_duration;
    public float end_scale;
    private Color goal_color;

    private void Awake() {
        var text_component = GetComponent<TextMeshProUGUI>();
        goal_color = text_component.color;
        text_component.color = new Color(goal_color.r, goal_color.g, goal_color.b, 0f);
    }

    public void Show(System.Action callback = null) {
        transform.DOScale(end_scale, show_duration).OnComplete(() => {
            callback?.Invoke();
        });
        var text_component = GetComponent<TextMeshProUGUI>();
        text_component.DOColor(goal_color, show_duration).SetEase(Ease.InCubic);
    }
}
