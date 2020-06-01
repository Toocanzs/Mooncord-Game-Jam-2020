using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour
{
    private float time = 0;
    public List<Image> images;
    [FormerlySerializedAs("curve")]
    public AnimationCurve reunitedCurve;
    public Image fade;

    void Start()
    {
    }

    void Update()
    {
        float v = reunitedCurve.Evaluate(time);
        foreach (var image in images)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, v);
        }
        fade.color = new Color(0,0,0, Mathf.Clamp01(1.3f-time/4));

        time += Time.deltaTime;
    }
}