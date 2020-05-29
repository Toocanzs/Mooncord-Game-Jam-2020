using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    public static Singleton<T> Instance;

    public virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            Debug.LogError($"Two instances of {this} found. This is supposed to be a singleton");
            return;
        }

        Instance = this;
    }
}
