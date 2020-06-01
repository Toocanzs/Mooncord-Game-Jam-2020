using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(HealthComponent))]
public class SoundOnDamage : MonoBehaviour
{
    public AudioClip sound;
    private AudioSource audioSource;
    private HealthComponent healthComponent;

    private bool died = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.on_health_change += HandleHealthChange;
    }

    private void HandleHealthChange(int difference)
    {
        audioSource.PlayOneShot(sound);
        if (healthComponent.Health <= 0)
        {
            healthComponent.on_health_change -= HandleHealthChange;
            return;
        }
    }

    void Update()
    {
        
    }
}
