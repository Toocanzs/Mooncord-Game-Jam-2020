using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EddieScript : MonoBehaviour
{
    public float movementSpeed = 10;

    private int phase = 0;

    public GameObject laserPrefab;
    public Transform laserSpawnPosition;
    public GameObject spikePrefab;
    public Transform spikeSpawnPoint;
    public GameObject minePrefab;

    public Transform pivot;

    public float heightBarrier = 44.48878f;

    private enum State
    {
        NONE,
        IDLE,
        Jumping,
        FIRING_LASER,
        FIRING_SPIKES,
        DEAD,
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

    public Transform outOfBoundsLaserSpawn;

    private HealthComponent healthComponent;
    
    private List<SpriteRenderer> sprite_renderers = new List<SpriteRenderer>();

    public AudioClip hitSound;

    public string EndSceneName = "Ending scene";

    void Start()
    {
        //ChangeState(State.IDLE);
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.on_health_change += OnHealthChange;
        sprite_renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    private void OnHealthChange(int difference)
    {
        if ((float) healthComponent.Health / healthComponent.max_health < 0.5f)
        {
            phase = 1;
        }
        if (healthComponent.Health <= 0)
        {
            //TODO: actually die
            ChangeState(State.DEAD);
            animator.SetTrigger("Dead");
            healthComponent.on_health_change -= OnHealthChange;
            float time = 0;
            for (int i = 0; i < 15; i++)
            {
                StartCoroutine(SpawnMine((Vector2) pivot.position + Random.insideUnitCircle * 5f, time));
                time += Random.Range(0.02f, 0.05f);
            }
            this.enabled = false;
            StartCoroutine(Die(1.5f));
        }
        else
        {
            sprite_renderers.ForEach((sr) => {
                DOTween.Kill(sr.material);
                sr.material.color = Color.black;
                sr.material.DOColor(Color.white, "_AdditiveColor", 0.4f).SetEase(Ease.Flash,8,1).OnComplete(() => {
                    sr.material.SetColor("_AdditiveColor", Color.black);
                });
            });
        }
        
        audioSource.PlayOneShot(hitSound);
    }
    
    IEnumerator Die(float time)
    {
        yield return new WaitForSeconds(time);
        transform.DOScale(Vector3.zero, 0.5f);
        GameCamera.GetCamera().EnableFade();
        GameCamera.GetCamera().StartFade(3.5f, 1f, () =>
        {
            SceneManager.LoadScene(EndSceneName, LoadSceneMode.Single);
        });
    }

    public void BeginJump()
    {
        TurnToFace();
        jumping = true;
        var pos = PlayerCharacter.GetPostion();
        pos.y = Mathf.Min(pos.y, heightBarrier);
        jumpTarget = pos - Vector3.Scale(pivot.localPosition, transform.localScale);
        jumpBegin = transform.position;
    }

    public void BeginShootSpikes()
    {
        TurnToFace();
        float angle = Vector2Extentions.GetAngle((PlayerCharacter.GetPostion() - laserSpawnPosition.position).normalized);
        int numSpikes = 6 + phase * 2;
        bool direct = Random.Range(0, 2) == 0;
        for (int i = 0; i < numSpikes; i++)
        {
            float percent = (float) i / numSpikes;
            percent = -1f + 2f * percent;
            float angleOffset = (percent * (numSpikes / 6f)) + Random.Range(-0.1f, 0.1f);
            if (i == numSpikes / 2)
                angleOffset = 0;
            quaternion rot = quaternion.Euler(0, 0, angle + angleOffset);
            float fireWaitTime = ((float) i / numSpikes) * (laserFireTime + numSpikes * 0.05f);

            StartCoroutine(FireSingleSpike(rot, fireWaitTime, direct));
        }
    }

    public void EndShootSpikes()
    {
        animator.ResetTrigger("Jump");
        ChangeState(State.IDLE, 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = transform.position;
        pos.y = heightBarrier;

        Gizmos.color = Color.blue;
        Gizmos.DrawCube(pos, new Vector3(150, 0.1f, 1));
    }

    public void EndJump()
    {
        animator.ResetTrigger("Jump");
        ChangeState(State.IDLE, 0.1f);
    }

    public void JumpSound()
    {
        audioSource.PlayOneShot(jumpSound);
    }

    public void FireLasers()
    {
        TurnToFace();
        float angle = Vector2Extentions.GetAngle((PlayerCharacter.GetPostion() - laserSpawnPosition.position).normalized);
        int lasersToShoot = numLasers + phase * 4;
        for (int i = 0; i < lasersToShoot; i++)
        {
            float percent = (float) i / lasersToShoot;
            percent = -1f + 2f * percent;
            float angleOffset = (percent * (lasersToShoot / 6f)) + Random.Range(-0.1f, 0.1f);
            if (i == lasersToShoot / 2)
                angleOffset = 0;
            quaternion rot = quaternion.Euler(0, 0, angle + angleOffset);
            float fireWaitTime = ((float) i / lasersToShoot) * (laserFireTime + lasersToShoot * 0.05f);

            StartCoroutine(FireSingleLaser(rot, fireWaitTime));
        }

        var pos = PlayerCharacter.GetPostion();
        if (pos.y > heightBarrier)
        {
            for (int i = 0; i < 5; i++)
            {
                StartCoroutine(FireTopLasers(new Vector3(0, -i, 0), i * 0.15f));
            }
        }
    }

    public void LandSound()
    {
        audioSource.PlayOneShot(landingSound, 0.3f);
    }

    public void EndLaser()
    {
        animator.ResetTrigger("Laser");
        ChangeState(State.IDLE, 0.1f);
    }

    void RandomFromIdle()
    {
        int numberOfMoves = 3;
        int value = Random.Range(0, numberOfMoves);
        switch (value)
        {
            case 0:
                ChangeState(State.Jumping);
                break;
            case 1:
                ChangeState(State.FIRING_LASER, 3);
                break;
            case 2:
                ChangeState(State.FIRING_SPIKES, 3);
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
                    if (Vector3.Distance(PlayerCharacter.GetPostion(), transform.position) > 12)
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

                    if (phase > 0 && Random.Range(0, 2) == 0)
                    {
                        Vector2 pos = PlayerCharacter.GetPostion();
                        Vector2 velocity = PlayerCharacter.GetVelocity();
                        Vector2 target = pos + velocity * Random.Range(0, 0.6f);
                        StartCoroutine(SpawnMine(target, 0.1f));
                    }
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
                    previousState = currentState;
                    animator.SetTrigger("Laser");
                    Invoke("EndLaser", (laserFireTime + numLasers * 0.05f) + 1.4f);
                }
            }
                break;
            case State.FIRING_SPIKES:
            {
                if (previousState != currentState)
                {
                    previousState = currentState;
                    animator.SetTrigger("Spike");
                }
            }
                break;
            case State.DEAD:
            {
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

    IEnumerator FireSingleSpike(quaternion dir, float time, bool direct)
    {
        yield return new WaitForSeconds(time);
        var go = Instantiate(spikePrefab, spikeSpawnPoint.position, dir);
        var script = go.GetComponent<Spike>();
        script.direct = direct;
        script.lockDistance *= direct ? 1 : 2;
    }
    
    IEnumerator SpawnMine(Vector2 position, float time)
    {
        yield return new WaitForSeconds(time);
        var go = Instantiate(minePrefab, position, Quaternion.identity);
    }

    IEnumerator FireTopLasers(Vector3 offset, float time)
    {
        yield return new WaitForSeconds(time);
        var go = Instantiate(laserPrefab, outOfBoundsLaserSpawn.position + offset, Quaternion.identity);
        go.GetComponent<LaserScript>().initialDirection = Vector3.right;
    }

    void ChangeState(State state, float targetTime = 0)
    {
        previousState = currentState;
        stateTransitionTime = targetTime;
        currentState = state;
        stateTime = 0;
    }
}