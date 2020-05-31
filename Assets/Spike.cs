using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public bool direct = true;

    private LineRenderer lineRenderer;
    private float leadTime = 0.5f;
    public float speed = 5f;
    public float rotationSpeed = 5f;

    public LayerMask collision;
    private Vector2 target;

    public float lockDistance = 2f;
    private bool locked = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        lineRenderer.SetPosition(0, (Vector2) transform.position);
        if (!locked)
        {
            target = PlayerCharacter.GetPostion();
            if (!direct)
            {
                target = target + PlayerCharacter.GetVelocity() * leadTime;
            }
        }
        
        if (Vector2.Distance(target, transform.position) < lockDistance)
        {
            locked = true;
            lineRenderer.enabled = false;
        }

        if (locked && Vector2.Distance(target, transform.position) < 0.2)
        {
            Destroy(gameObject);
        }

        lineRenderer.SetPosition(1, target);

        float angle = Vector2Extentions.GetAngle((target - (Vector2) transform.position).normalized);

        if (locked)
        {
            rotationSpeed += Time.deltaTime * 50;
        }
        
        transform.rotation = Quaternion.RotateTowards(transform.rotation, quaternion.Euler(0, 0, angle), Time.deltaTime * rotationSpeed);
        transform.position += Time.deltaTime * speed * transform.right;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent(typeof(PlayerCharacter), out var character))
        {
            character.GetComponent<HealthComponent>().ChangeHealth(-2);
            Destroy(gameObject);
        }
    }
}