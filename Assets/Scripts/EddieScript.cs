using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EddieScript : MonoBehaviour
{
    public float movementSpeed = 10;

    private int phase = 0;

    public GameObject laserPrefab;
    public Transform laserSpawnPosition;

    private enum State
    {
        IDLE,
        RUNNING,
        FIRING_LASER
    }

    private State currentState = State.IDLE;

    private float stateTime = 0;
    private float stateTransitionTime = 0;

    public int numLasers = 4;
    public float laserFireTime = 0.5f;

    void Start()
    {
        ChangeState(State.IDLE);
    }
    
    void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
            {
                if (stateTransitionTime == 0)
                {
                    stateTransitionTime = Random.Range(1f, 2f);
                }

                if (stateTime > stateTransitionTime)
                {
                    ChangeState(State.FIRING_LASER, 3);
                }
            } break;
            case State.RUNNING:
            {
                
            } break;
            case State.FIRING_LASER:
            {
                float angle = Vector2Extentions.GetAngle((PlayerCharacter.GetPostion() - laserSpawnPosition.position).normalized);
                for (int i = 0; i < numLasers; i++)
                {
                    float percent = (float) i / numLasers;
                    percent = -1f + 2f * percent;
                    float angleOffset = (percent * 0.6f) + Random.Range(-0.1f, 0.1f);
                    if (i == numLasers / 2)
                        angleOffset = 0;
                    quaternion rot = quaternion.Euler(0,0, angle + angleOffset);
                    float fireWaitTime = ((float)i /numLasers) * (laserFireTime + numLasers * 0.05f);
                    
                    StartCoroutine(FireSingleLaser(rot, fireWaitTime));
                }
                ChangeState(State.IDLE, laserFireTime  + numLasers * 0.05f + Random.Range(0.5f,1f));
            } break;
            default:
                Debug.LogError($"Unhandled state {currentState}");
                break;
        }

        stateTime += Time.deltaTime;
    }

    IEnumerator FireSingleLaser(quaternion dir, float time)
    {
        yield return new WaitForSeconds(time);
        var go = Instantiate(laserPrefab, laserSpawnPosition.position, Quaternion.identity, laserSpawnPosition);
        go.GetComponent<LaserScript>().initialDirection = math.mul(dir, Vector3.right).xy;
    }

    void ChangeState(State state, float targetTime = 0)
    {
        stateTransitionTime = targetTime;
        currentState = state;
        stateTime = 0;
    }
}
