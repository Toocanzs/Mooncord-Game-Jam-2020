using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeParticles : MonoBehaviour
{

    private Transform followTarget;
    private bool stopped = false;
    void Start()
    {
        followTarget = transform.parent;
        transform.parent = null;
    }

    void Update()
    {
        if (followTarget == null)
        {
            if (!stopped)
            {
                stopped = true;
                var emis = GetComponent<ParticleSystem>().emission;
                emis.enabled = false;
                Destroy(gameObject, 5);
            }
        }
        else
        {
            transform.position = followTarget.position;
        }
        
    }
}
