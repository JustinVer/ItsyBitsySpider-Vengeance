using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Beetle : EnemyBase, IFireAnimation
{
    private SendFireSignal fireSignal;
    private bool isFiring = false;
    private bool isJumping = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float ProjectileVelocity = 10f;
    [SerializeField] private Transform fireLocation;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float timeDistanceMultiplier = 0.05f;
    [SerializeField] private float timeDistanceBase = 0.25f;
    private float distanceToPlayer = 999f;
    [SerializeField] private float maxDegreesRotationNormal = 90f;
    [SerializeField] private float maxDegreesRotationJump = 90f;
    Rigidbody rb;
    [SerializeField] private Platform[] platforms;
    private Platform currentPlatformScript = null;
    private Transform currentPlatformTransform = null;
    [SerializeField] private float detectionWidth = 2f;
    [SerializeField] private int numDetectionCasts = 11;
    [SerializeField] private float fireVelocityDistanceMultiplier = 0.5f;
    private float landTime = 0.95f;

    [SerializeField] private AudioClip jump;
    [SerializeField, Range(0, 1)] private float jumpVolume = 0.5f;
    [SerializeField] private AudioClip shoot;
    [SerializeField, Range(0, 1)] private float shootVolume = 0.5f;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        base.Awake();
    }

    private void Start()
    {
        fireSignal = this.gameObject.GetComponentInChildren<SendFireSignal>();
        fireSignal.body = this;
        platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);
    }

    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, GameplayManager.Instance.PlayerBody.transform.position);
        if (!isJumping)
        {
            rotateTowards(fireLocation.position, getPlayerProjectionPosition(), GameplayManager.Instance.GetGravity(this.transform.position) * -1, maxDegreesRotationNormal);
        }
        base.NotDyingUpdate();
    }

    private Vector3 getPlayerProjectionPosition()
    {
        return GameplayManager.Instance.PlayerBody.transform.position + ((GameplayManager.Instance.PlayerBody.LinearVelocity() * fireVelocityDistanceMultiplier * Mathf.Abs(Vector3.Distance(GameplayManager.Instance.PlayerBody.transform.position, this.transform.position))) / ProjectileVelocity);
    }
    public override void EndDeath()
    {


        this.gameObject.SetActive(false);
        ReturnSelf();
    }

    public void FireProjectile()
    {
        if (isDying || isJumping) return;
        GameObject bullet = GameObject.Instantiate(projectilePrefab);
        bullet.transform.forward = fireLocation.forward;
        bullet.transform.position = fireLocation.position;
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = fireLocation.forward * ProjectileVelocity;
        Debug.Log("fire projectile");
    }

    public void FireComplete()
    {
        isFiring = false;
    }

    public override void ReturnSelf()
    {
        if (currentPlatformScript != null && currentPlatformTransform != null)
        {
            currentPlatformScript.returnPlatformPoint(currentPlatformTransform);
        }
        isFiring = false;
        isJumping = false;
        setHP(data.maxHP);
        StopAllCoroutines();
        parentPool.Return(this);
    }

    protected override void Attack()
    {
        Debug.Log("Beetle fire " + isFiring + " " + isJumping);
        if (!isFiring && !isJumping && distanceToPlayer < data.attackRange)
        {
            Debug.Log("start fire projectile");
            animator.SetTrigger("Fire1");
            isFiring = true;
        }
        AudioManager.Instance.PlaySound(shoot, shootVolume, transform.position);
    }

    protected override void Die()
    {
        if (!isDying)
        {
            AudioManager.Instance.PlaySound(death, deathVolume, transform.position);
            if (currentPlatformScript != null && currentPlatformTransform != null)
            {
                currentPlatformScript.returnPlatformPoint(currentPlatformTransform);
            }
            GameplayManager.Instance.PlayerBody.CurrentWebs++;
            GameplayManager.Instance.score++;
            animator.SetTrigger("Died");
            isDying = true;
        }
    }

    protected override void Move()
    {
        if (!isDying && !isJumping)
        {
            //Debug.Log("Mpve beetle " + (distanceToPlayer < data.detectionDistanceClose));
            if (distanceToPlayer < data.detectionDistanceClose)
            {
                Vector3 endPosition = getNewPlatformPosition();
                if (endPosition != this.transform.position)
                {
                    StartCoroutine(Jump(endPosition, jumpHeight, (Vector3.Distance(this.transform.position, endPosition) * timeDistanceMultiplier) + timeDistanceBase));
                }
            }
            else
            {
                rb.AddForce(GameplayManager.Instance.GetGravity(this.transform.position).normalized * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator Jump(Vector3 endPos, float height, float duration)
    {
        //Debug.Log("Start Jump beetle " + endPos + " " + height + " " + duration);
        isJumping = true;
        Vector3 startPos = this.transform.position;
        float normalizedTime = 0.0f;
        Vector3 gravityDir = -GameplayManager.Instance.GetGravity(Vector3.Lerp(startPos, endPos, 0.5f)).normalized;
        yield return new WaitForFixedUpdate();
        animator.SetBool("Jumping", true);
        AudioManager.Instance.PlaySound(jump, jumpVolume, transform.position);
        while (normalizedTime < 1.0f)
        {
            animator.SetFloat("JumpT", normalizedTime);
            rotateTowards(endPos, getPlayerProjectionPosition(), -GameplayManager.Instance.GetGravity(endPos), maxDegreesRotationJump);
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            this.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * gravityDir;
            normalizedTime += Time.deltaTime / duration;
            if (normalizedTime * duration > duration - landTime)
            {
                animator.SetBool("Landing", true);
            }
            yield return new WaitForFixedUpdate();
        }
        isJumping = false;
        animator.SetBool("Jumping", false);
        animator.SetBool("Landing", false);
    }

    private Vector3 getNewPlatformPosition()
    {
        if (currentPlatformScript != null && currentPlatformTransform != null)
        {
            currentPlatformScript.returnPlatformPoint(currentPlatformTransform);
        }

        MaxHeap<Platform> maxHeap = new MaxHeap<Platform>(platforms, x => platformHeuristic(x));

        Platform currentPlatform = null;
        while (maxHeap.Count > 0)
        {
            currentPlatform = maxHeap.Pull();
            List<Transform> platformTransforms = new List<Transform>();
            Transform currentTransform = currentPlatform.getPlatformPoint();

            while (currentTransform != null)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(currentTransform.position - GameplayManager.Instance.GetGravity(currentTransform.position).normalized * 2, out hit, 5.0f, NavMesh.AllAreas))
                {
                    Vector3 platformPosition = hit.position;
                    if (!obstaclesInJump(this.transform.position, platformPosition, 3f, 2f, currentPlatform.PlatformObject))
                    {
                        foreach (Transform t in platformTransforms)
                        {
                            currentPlatform.returnPlatformPoint(t);
                        }
                        return platformPosition;
                    }
                }

                platformTransforms.Add(currentTransform);
                currentTransform = currentPlatform.getPlatformPoint();
            }

            foreach (Transform t in platformTransforms)
            {
                currentPlatform.returnPlatformPoint(t);
            }

        }
        return this.transform.position;
    }

    private float platformHeuristic(Platform platform)
    {

        float heuristic = 0;

        try
        {
            heuristic += (Vector3.Distance(this.transform.position, platform.PlatformObject.transform.position) - 5f);
            heuristic += UnityEngine.Random.Range(-10, 10);
        }
        catch (Exception)
        {

        }

        return heuristic;
    }

    private bool obstaclesInJump(Vector3 startPos, Vector3 endPos, float height, float duration, GameObject platform)
    {
        Vector3 gravityDir = -GameplayManager.Instance.GetGravity(Vector3.Lerp(startPos, endPos, 0.5f)).normalized;

        Vector3 halfExtents = new Vector3(1f, 1f, 1f); // match beetle size

        for (int i = 1; i <= numDetectionCasts - 1; i++)
        {
            float t = i / (float)numDetectionCasts;

            float yOffset = height * 4.0f * (t - t * t);
            Vector3 samplePos = Vector3.Lerp(startPos, endPos, t) + yOffset * gravityDir;

            //collision check
            Collider[] hits = Physics.OverlapBox(samplePos, halfExtents, Quaternion.identity, GameplayManager.Instance.NotPlayerOrEnemyMask);

            if (hits.Length > 0 && hits[0].gameObject != platform)
            {
                return true;
            }
        }

        return false;
    }

    private void rotateTowards(Vector3 startPosition, Vector3 endPosition, Vector3 gravity, float maxRotation)
    {
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(endPosition - startPosition, gravity), maxRotation * Time.fixedDeltaTime);
    }
}
