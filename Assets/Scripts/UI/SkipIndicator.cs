using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkipIndicator : MonoBehaviour
{
    public Image progress_indicator;
    public TextMeshProUGUI hold_text;
    public TextMeshProUGUI skip_text;

    public void SetProgress(float percent) {
        if(percent > 0f) {
            hold_text.alpha = 0f;
            skip_text.alpha = 0f;
        } else {
            hold_text.alpha = 1f;
            skip_text.alpha = 1f;
        }
        progress_indicator.fillAmount = percent;
    }

}
