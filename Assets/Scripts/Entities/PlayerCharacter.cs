using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerCharacter : Character
{
    private static PlayerCharacter Instance;

    public static Vector3 GetPostion()
    {
        if (Instance == null)
            return Vector3.zero;
        return Instance.transform.position;
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError("Two players existed in the scene, destroying.");
            return;
        }

        Instance = this;
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

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnDeath() {
        base.OnDeath();
    }
}
