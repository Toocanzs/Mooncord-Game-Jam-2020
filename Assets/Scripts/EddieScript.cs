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

    public Transform pivot;

    private enum State
    {
        NONE,
        IDLE,
        Jumping,
        FIRING_LASER,
        TARGETING
    }

    private State currentState = State.IDLE;
    private State previousState = State.NONE;

    private float stateTime = 0;
    private float stateTransitionTime = 0;

    public int numLasers = 4;
    public float laserFireTime = 0.5f;

    public int numberOfTargets = 1;
    //public bool 

    private float jumpTime = 0;
    private bool jumping = false;
    public float jumpAnimSpeed = 1f;

    private Animator animator;

    private Vector3 jumpBegin = Vector3.zero;
    private Vector3 jumpTarget = Vector3.zero;

    public AudioClip jumpSound;
    public AudioClip landingSound;

    private AudioSource audioSource;

    void Start()
    {
        //ChangeState(State.IDLE);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void BeginJump()
    {
        TurnToFace();
        jumping = true;
        jumpTarget = PlayerCharacter.GetPostion() - Vector3.Scale(pivot.localPosition, transform.localScale);
        jumpBegin = transform.position;
    }

    public void EndJump()
    {
        animator.ResetTrigger("Jump");
        ChangeState(State.IDLE, 0.4f);
    }

    public void JumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }
    
    public void LandSound()
    {
        audioSource.PlayOneShot(landingSound, 0.3f);
    }

    public void EndLaser()
    {
        Debug.Log("Stopped laser");
        animator.ResetTrigger("Laser");
        ChangeState(State.IDLE, 0.4f);
    }

    void RandomFromIdle()
    {
        int value = Random.Range(0, 2);
        switch (value)
        {
            case 0:
                ChangeState(State.FIRING_LASER, 3);
                break;
            case 1:
                ChangeState(State.Jumping);
                break;
            default:
                Debug.LogError($"Unhandled random change from idle {value}");
                break;
        }
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
                    if (Vector3.Distance(PlayerCharacter.GetPostion(), transform.position) > 10)
                    {
                        ChangeState(State.Jumping);
                    }
                    else
                    {
                        RandomFromIdle();
                    }
                }
            }
                break;
            case State.Jumping:
            {
                if (previousState != currentState)
                {
                    previousState = currentState;
                    jumpTime = 0;
                    jumping = false;
                    animator.SetTrigger("Jump");
                }

                if (jumping)
                {
                    jumpTime += Time.deltaTime * jumpAnimSpeed;
                    transform.position = Vector3.Lerp(jumpBegin, jumpTarget, jumpTime);
                }
            }
                break;
            case State.FIRING_LASER:
            {
                if (previousState != currentState)
                {
                    TurnToFace();
                    previousState = currentState;
                    float angle = Vector2Extentions.GetAngle((PlayerCharacter.GetPostion() - laserSpawnPosition.position).normalized);
                    animator.SetTrigger("Laser");
                    for (int i = 0; i < numLasers; i++)
                    {
                        float percent = (float) i / numLasers;
                        percent = -1f + 2f * percent;
                        float angleOffset = (percent * (numLasers / 6f)) + Random.Range(-0.1f, 0.1f);
                        if (i == numLasers / 2)
                            angleOffset = 0;
                        quaternion rot = quaternion.Euler(0, 0, angle + angleOffset);
                        float fireWaitTime = ((float) i / numLasers) * (laserFireTime + numLasers * 0.05f);

                        StartCoroutine(FireSingleLaser(rot, fireWaitTime));
                    }
                    Invoke("EndLaser", (laserFireTime + numLasers * 0.05f) + 1.4f);
                }
            }
                break;
            case State.TARGETING:
            {
                ChangeState(State.IDLE, laserFireTime + numLasers * 0.05f + Random.Range(0.5f, 1f));
            }
                break;
            default:
                Debug.LogError($"Unhandled state {currentState}");
                break;
        }

        stateTime += Time.deltaTime;
    }

    void TurnToFace()
    {
        Vector2 dif = pivot.position - PlayerCharacter.GetPostion();
        float d = Vector2.Dot(dif, Vector2.left);
        if (d <= 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    IEnumerator FireSingleLaser(quaternion dir, float time)
    {
        yield return new WaitForSeconds(time);
        var go = Instantiate(laserPrefab, laserSpawnPosition.position, Quaternion.identity, laserSpawnPosition);
        go.GetComponent<LaserScript>().initialDirection = math.mul(dir, Vector3.right).xy;
    }

    void ChangeState(State state, float targetTime = 0)
    {
        previousState = currentState;
        stateTransitionTime = targetTime;
        currentState = state;
        stateTime = 0;
    }
}