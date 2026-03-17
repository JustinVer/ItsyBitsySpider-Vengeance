using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BigT : MonoBehaviour, IFireAnimation, IDamageable
{
    [SerializeField] private AgentLinkMover agentMover;
    [SerializeField] private BodyFollowAgent bodyFollower;
    private float distanceToPlayer = 999f;
    [SerializeField] private State currentState = State.Attack;
    private bool trySummoning = false;
    [SerializeField] private Platform[] platforms;
    private Platform currentPlatformScript = null;
    private Transform currentPlatformTransform = null;
    [SerializeField] private float detectionWidth = 2f;
    [SerializeField] private int numDetectionCasts = 11;
    [SerializeField] private float jumpHeight = 4.0f;
    [SerializeField] private float timeDistanceMultiplier = 0.05f;
    [SerializeField] private float timeDistanceBase = 0.25f;
    [SerializeField] private float maxDegreesRotationJump = 90f;
    [SerializeField] private float distancePastBossJumps = 40f;

    [SerializeField] private SendFireSignal fireSignal;
    private bool isFiring = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float ProjectileVelocity = 10f;
    [SerializeField] private Transform fireLocation;
    private bool isDying = false;
    [SerializeField] private float closestDistanceMoveToPlayer = 15f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float chaseRange = 25f;
    [SerializeField] private Animator animator;

    [SerializeField] private float minTimeBetweenSummons = 30.0f;
    [SerializeField] private float timeSinceLastSummon = 0.0f;

    [SerializeField] private int baseNumMinionsPerSummon = 4;
    [SerializeField] private int numMinionsPerSummonIncrease = 1;
    [SerializeField] private int randomnessOfMinionsPerSummon = 2;

    [SerializeField] private int maxHitPoint = 500;
    private int currentHP = 500;

    [SerializeField] private ParticleSystem damageParticle;
    [SerializeField] private float maxRotation = 90f;
    [SerializeField] private float landTime = 0.75f;
    [SerializeField] private BigTGun gun;

    private enum State
    {
        Summon,
        Jump,
        Attack
    }

    private void Awake()
    {
        currentHP = maxHitPoint;
        if (damageParticle)
        {
            damageParticle.transform.parent = null;
            damageParticle.gameObject.SetActive(true);
        }
    }

    private void Start()
    {
        fireSignal = this.gameObject.GetComponentInChildren<SendFireSignal>();
        fireSignal.body = this;
        platforms = FindObjectsByType<Platform>(FindObjectsSortMode.None);
    }


    private void Update()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position);
        timeSinceLastSummon += Time.deltaTime;
        switch (currentState)
        {
            case State.Summon:
                updateSummon();
                break;
            case State.Jump:
                break;
            case State.Attack:
                updateAttack();
                break;
        }
    }

    private void updateSummon()
    {
        if (trySummoning && canSummon())
        {
            bodyFollower.Anim.SetTrigger("Summon");
            BossFightManager.Instance.SummonRandomEnemies(baseNumMinionsPerSummon + (int)((Random.value * randomnessOfMinionsPerSummon) - (int)(randomnessOfMinionsPerSummon * 0.5f)));
            baseNumMinionsPerSummon += numMinionsPerSummonIncrease;
            trySummoning = false;
        }
        else if (!trySummoning && !bodyFollower.Anim.GetCurrentAnimatorStateInfo(0).IsName("Summon"))
        {
            nextState();
        }
    }

    private bool canSummon()
    {

        if (Physics.Raycast(bodyFollower.transform.position, GameplayManager.Instance.GetGravity(bodyFollower.transform.position), 3, GameplayManager.Instance.NotPlayerOrEnemyMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    private bool startJump()
    {
        Vector3 endPosition = getNewPlatformPosition();
        if (endPosition != this.transform.position)
        {
            StartCoroutine(Jump(endPosition, jumpHeight, (Vector3.Distance(this.transform.position, endPosition) * timeDistanceMultiplier) + timeDistanceBase));
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator Jump(Vector3 endPos, float height, float duration)
    {
        currentState = State.Jump;
        animator.SetBool("Jumping", true);
        Vector3 startPos = this.transform.position;
        float normalizedTime = 0.0f;
        Vector3 gravityDir = -GameplayManager.Instance.GetGravity(Vector3.Lerp(startPos, endPos, 0.5f)).normalized;
        yield return new WaitForFixedUpdate();
        while (normalizedTime < 1.0f)
        {
            //rotateTowards(endPos, getPlayerProjectionPosition(), -GameplayManager.Instance.GetGravity(endPos), maxDegreesRotationJump);
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            this.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * gravityDir;
            normalizedTime += Time.deltaTime / duration;
            if (normalizedTime * duration > duration - landTime)
            {
                animator.SetBool("Landing", true);
            }
            rotateTowards(startPos, endPos, -GameplayManager.Instance.GetGravity(bodyFollower.transform.position), maxRotation);
            yield return new WaitForFixedUpdate();
        }
        animator.SetBool("Jumping", false);
        nextState();
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
            if (platformHeuristic(currentPlatform) < 0)
            {
                return this.transform.position;
            }
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

        heuristic += (Vector3.Distance(this.transform.position, platform.PlatformObject.transform.position) - 5f);
        heuristic += UnityEngine.Random.Range(-10, 10);

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

    private void updateAttack()
    {
        if (!isFiring)
        {
            if (distanceToPlayer >= chaseRange)
            {
                nextState();
            }
            if (distanceToPlayer < attackRange)
            {
                animator.SetTrigger("Fire1");
                Debug.Log("Boss fire start");
                isFiring = true;
            }

        }
        if (distanceToPlayer > closestDistanceMoveToPlayer)
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
        }
        else
        {
            agentMover.agent.velocity = Vector3.zero;
            bodyFollower.RB.linearVelocity = Vector3.zero;
            bodyFollower.RB.angularVelocity = Vector3.zero;
        }
        rotateTowardsPlayerAndBody(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, -GameplayManager.Instance.GetGravity(bodyFollower.transform.position), maxRotation);
    }

    public void FireProjectile()
    {
        if (isDying || currentState == State.Jump) return;
        gun.Shoot();
    }

    public void FireComplete()
    {
        isFiring = false;
    }

    private void nextState()
    {
        if (currentState == State.Jump)
        {
            if (distanceToPlayer >= distancePastBossJumps)
            {
                currentState = State.Summon;
            }
            else
            {
                currentState = State.Attack;
            }
        }
        else if (currentState == State.Summon)
        {
            if (distanceToPlayer >= distancePastBossJumps && startJump())
            {
                currentState = State.Jump;
            }
            else
            {
                currentState = State.Attack;
            }
        }
        else if (currentState == State.Attack)
        {
            if (timeSinceLastSummon >= minTimeBetweenSummons)
            {
                currentState = State.Summon;
            }
            else if (distanceToPlayer >= distancePastBossJumps && startJump())
            {
                currentState = State.Jump;
            }
            else
            {
                currentState = State.Attack;
            }
        }
    }

    public float getHP()
    {
        return currentHP;
    }

    public void modifyHP(int hpChange)
    {
        currentHP = (int)Mathf.Clamp(currentHP + hpChange, 0f, maxHitPoint);
        if (currentHP <= 1.0f)
        {
            Die();
        }
    }

    public void setHP(int hp)
    {
        currentHP = (int)Mathf.Clamp(hp, 0f, maxHitPoint);
    }

    private void Die()
    {

    }

    public void hitEffect(Vector3 position, Vector3 forwardDirection)
    {
        damageParticle.transform.position = position;
        damageParticle.transform.forward = forwardDirection;
        damageParticle.Play();
    }

    private void rotateTowardsPlayerAndBody(Vector3 startPosition, Vector3 endPosition, Vector3 gravity, float maxRotation)
    {
        bodyFollower.transform.rotation = Quaternion.RotateTowards(bodyFollower.transform.rotation, Quaternion.LookRotation(endPosition - startPosition, gravity), maxRotation * Time.fixedDeltaTime);
        Quaternion rotation2 = Quaternion.RotateTowards(bodyFollower.transform.rotation, agentMover.transform.rotation, maxRotation * Time.fixedDeltaTime);
        Vector3 rot = bodyFollower.transform.localEulerAngles;
        rot.x = rotation2.eulerAngles.x;
        rot.z = rotation2.eulerAngles.z;
        bodyFollower.transform.localEulerAngles = rot;
    }

    private void rotateTowards(Vector3 startPosition, Vector3 endPosition, Vector3 gravity, float maxRotation)
    {
        bodyFollower.transform.rotation = Quaternion.RotateTowards(bodyFollower.transform.rotation, Quaternion.LookRotation(endPosition - startPosition, gravity), maxRotation * Time.fixedDeltaTime);
    }
}
