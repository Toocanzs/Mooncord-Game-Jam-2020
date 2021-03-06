﻿using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float laserWidth = 1;
    public float animationSpeed = 0.5f;
    public AnimationCurve laserCurve;
    public LayerMask wallLayers;
    private float time = 0;

    public GameObject hurtPrefab;
    private bool didHurt = false;

    public Vector2 initialDirection;
    
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        var hit = Physics2D.Raycast(transform.position, initialDirection, float.PositiveInfinity, wallLayers);
        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        
        float eval = laserCurve.Evaluate(time);
        Vector3 otherPos = lineRenderer.GetPosition(1);
        Vector2 dif = otherPos - transform.position;
        var angle = Vector2Extentions.GetAngle(dif);

        transform.rotation = quaternion.Euler(0, 0, angle);

        if (eval > 0.7 && !didHurt)
        {
            didHurt = true;
            var go = Instantiate(hurtPrefab, transform.position, quaternion.Euler(0, 0, angle));
            //TODO: add damage to hurt prefab
            go.transform.localScale = new Vector3(dif.magnitude, 1, 1);
            Destroy(go, 0.1f);
        }
        else
        {
            lineRenderer.SetPosition(0, (Vector2)transform.position);
        }
        
        
        var width = eval * laserWidth;
        lineRenderer.widthMultiplier = width;
        if(time > 1f)
            Destroy(gameObject);
        time += Time.deltaTime * animationSpeed;
    }
}
