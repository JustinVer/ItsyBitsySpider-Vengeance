using System.Collections;
using UnityEngine;

public class Beetle : EnemyBase, IFireAnimation
{
    private SendFireSignal fireSignal;
    private bool isFiring = false;
    private bool isJumping = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float ProjectileVelocity = 10f;
    [SerializeField] private Transform fireLocation;
    private bool inJump = false;
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private float timeDistanceMultiplier = 0.05f;
    [SerializeField] private float timeDistanceBase = 0.25f;
    private float distanceToPlayer = 999f;
    [SerializeField] private float maxDegreesRotation = 90f;
    Rigidbody rb;

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
        rotateToPlayer();
        base.NotDyingUpdate();
    }
    public override void EndDeath()
    {
        ReturnSelf();
    }

    public void FireProjectile()
    {
        if (isDying) return;
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
        if (!isFiring && distanceToPlayer < data.attackRange && Physics.Linecast(this.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask))
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
            animator.SetTrigger("Died");
            isDying = true;
        }
    }

    protected override void Move()
    {
        if (!isDying && !inJump)
        {
            if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(this.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
            {
                Vector3 endPosition = getNewPlatformPosition();
                StartCoroutine(Parabola(endPosition, jumpHeight, (Vector3.Distance(this.transform.position, endPosition) * timeDistanceMultiplier) + timeDistanceBase));
            }
            else
            {
                rb.AddForce(GameplayManager.Instance.GetGravity(this.transform.position).normalized * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator Parabola(Vector3 endPos, float height, float duration)
    {
        inJump = true;
        Vector3 startPos = this.transform.position;
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            this.transform.position = Vector3.Lerp(startPos, endPos, normalizedTime) + yOffset * (-1 * GameplayManager.Instance.GetGravity(this.transform.position).normalized);
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        inJump = false;
    }

    private Vector3 getNewPlatformPosition()
    {
        //TODO implement platforming detection
        return this.transform.position;
    }

    private void rotateToPlayer()
    {
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, Quaternion.LookRotation(GameplayManager.Instance.PlayerBody.transform.position - fireLocation.position, GameplayManager.Instance.GetGravity(this.transform.position) * -1), maxDegreesRotation * Time.fixedDeltaTime);
    }
}
