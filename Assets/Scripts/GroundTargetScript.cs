using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GroundTargetScript : MonoBehaviour
{
    public AnimationCurve sizeCurve;
    public float rotationSpeed = 1f;
    public float animationSpeed = 0.5f;

    private float time = 0;
    void Start()
    {
        
    }
    
    void Update()
    {
        float value = sizeCurve.Evaluate(time);

        transform.localScale = Vector3.one * value;
        transform.rotation *= quaternion.Euler(0,0, rotationSpeed * Time.deltaTime);

        if(time > 1f)
            Destroy(gameObject);
        time += Time.deltaTime * animationSpeed;
    }
}
