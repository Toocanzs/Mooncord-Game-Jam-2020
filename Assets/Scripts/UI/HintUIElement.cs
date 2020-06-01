using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum HintType { SPRINT, EXITS, ITEM_GREASE }

public class HintUIElement : MonoBehaviour
{
    public HintType hint_type;
    public void Show(float time, System.Action callback = null) {
        var canvas_group = GetComponent<CanvasGroup>();
        var seq = DOTween.Sequence();
        seq.Append(canvas_group.DOFade(1.0f, time));
        seq.Insert(time + 2.0f, canvas_group.DOFade(0.0f, time));
        seq.OnComplete(() => {
            callback?.Invoke();
        });
    }
}
