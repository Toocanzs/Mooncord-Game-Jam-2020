﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCharacter : Character
{
    private static PlayerCharacter Instance;
    private bool disable_update;

    private Rigidbody2D rigidbody2D;

    public static Vector2 GetVelocity()
    {
        if (Instance == null)
            return Vector2.zero;
        return Instance.rigidbody2D.velocity;
    }

    public static Vector3 GetPostion()
    {
        if (Instance == null)
            return Vector3.zero;
        return Instance.transform.position;
    }

    protected override void Awake()
    {
        base.Awake();
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("Two players existed in the scene, destroying.");
            return;
        }

        Instance = this;

        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    public static PlayerCharacter GetPlayerCharacter() {
        if (!Instance) {
            Debug.LogError("Trying to get PlayerCharacter Instance that is null!");
        }
        return Instance;

    }

    public Transform GetTextBubbleAnchor() {
        return transform.Find("text_bubble_anchor");
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        if (disable_update) {
            return;
        }
        if (health_component.isDead()) {
            OnDeath();
        }
        
    }

    protected override void OnDeath() {
        // @TODO: animation
        var movement = GetComponent<PlayerMovement>();
        movement.StopMovement();
        ControlManager.SetInputEnabled(false);
        var game_ui = FindObjectOfType<GameUI>();
        game_ui.SetPlayerDeath();
        disable_update = true;
        base.OnDeath();
    }
}
