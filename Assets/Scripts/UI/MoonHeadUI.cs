using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MoonHeadUI : MonoBehaviour
{
    public void FadeIn(float time, System.Action callback = null) {
        var group = GetComponent<CanvasGroup>();
        group.DOFade(1.0f, time).OnComplete(() => {
            callback?.Invoke();
        });
    }
}

