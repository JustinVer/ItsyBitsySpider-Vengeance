using System.Collections;
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
    [SerializeField] private GameObject[] platforms;
    [SerializeField] private float detectionWidth = 2f;
    [SerializeField] private int numDetectionCasts = 11;
    [SerializeField] private float fireVelocityDistanceMultiplier = 0.5f;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        base.Awake();
    }

    private void Start()
    {
        fireSignal = this.gameObject.GetComponentInChildren<SendFireSignal>();
        fireSignal.body = this;
    }

    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(this.transform.position, GameplayManager.Instance.Player.transform.position);
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
        isFiring = false;
        isJumping = false;
        setHP(data.maxHP);
        StopAllCoroutines();
        parentPool.Return(this);
    }

    protected override void Attack()
    {
        if (!isFiring && !isJumping && distanceToPlayer < data.attackRange && Physics.Linecast(this.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask))
        {
            Debug.Log("start fire projectile");
            animator.SetTrigger("Fire1");
            isFiring = true;
        }
    }

    protected override void Die()
    {
        if (!isDying)
        {
            this.gameObject.SetActive(false);
            animator.SetTrigger("Died");
            isDying = true;
        }
    }

    protected override void Move()
    {
        if (!isDying && !isJumping)
        {
            Debug.Log("Mpve beetle " + (distanceToPlayer < data.detectionDistanceClose));
            if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(this.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
            {
                Vector3 endPosition = getNewPlatformPosition();
                StartCoroutine(Jump(endPosition, jumpHeight, (Vector3.Distance(this.transform.position, endPosition) * timeDistanceMultiplier) + timeDistanceBase));
            }
            else
            {
                rb.AddForce(GameplayManager.Instance.GetGravity(this.transform.position).normalized * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator Jump(Vector3 endPos, float height, float duration)
    {
        Debug.Log("Start Jump beetle " + endPos + " " + height + " " + duration);
        isJumping = true;
        Vector3 startPos = this.transform.position;
        float normalizedTime = 0.0f;
        Vector3 gravityDir = -GameplayManager.Instance.GetGravity(Vector3.Lerp(startPos, endPos, 0.5f)).normalized;
        yield return new WaitForFixedUpdate();
        while (normalizedTime < 1.0f)
        {
            rotateTowards(endPos, getPlayerProjectionPosition(), -GameplayManager.Instance.GetGravity(endPos), maxDegreesRotationJump);
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            this.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * gravityDir;
            Debug.Log("Beetle gravity " + GameplayManager.Instance.GetGravity(this.transform.position).normalized);
            normalizedTime += Time.deltaTime / duration;
            yield return new WaitForFixedUpdate();
        }
        isJumping = false;
    }

    private Vector3 getNewPlatformPosition()
    {
        MaxHeap<GameObject> maxHeap = new MaxHeap<GameObject>(platforms, x => platformHeuristic(x));

        GameObject currentPlatform = null;
        Debug.Log("beetle before while loop");
        while (maxHeap.Count > 0)
        {
            currentPlatform = maxHeap.Pull();
            Debug.Log("Beetle before obstacle check");
            NavMeshHit hit;
            if (NavMesh.SamplePosition(currentPlatform.transform.position - GameplayManager.Instance.GetGravity(currentPlatform.transform.position).normalized * 2, out hit, 5.0f, NavMesh.AllAreas))
            {
                Vector3 platformPosition = hit.position;
                if (!obstaclesInJump(this.transform.position, platformPosition, 3f, 2f, currentPlatform))
                {
                    Debug.Log("Beetle found platform");
                    return platformPosition;
                }
            }


        }
        return this.transform.position;
    }

    private float platformHeuristic(GameObject platform)
    {
        float heuristic = 0;

        heuristic += (Vector3.Distance(this.transform.position, platform.transform.position) - 5f);
        heuristic += Random.Range(-10, 10);

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
            Debug.Log("Beetle y offset" + yOffset + " " + gravityDir);
            Vector3 samplePos = Vector3.Lerp(startPos, endPos, t) + yOffset * gravityDir;

            //collision check
            Collider[] hits = Physics.OverlapBox(samplePos, halfExtents, Quaternion.identity, GameplayManager.Instance.NotPlayerOrEnemyMask);

            if (hits.Length > 0 && hits[0].gameObject != platform)
            {
                Debug.Log("Beetle jump blocked by " + hits[0].name);
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
