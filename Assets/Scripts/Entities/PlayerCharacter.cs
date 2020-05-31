using System;
using System.Collections;
using System.Collections.Generic;
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
    // Start is called before the first frame update
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
