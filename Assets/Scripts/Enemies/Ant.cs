using System.Collections;
using UnityEngine;

public class Ant : EnemyBase, IFireAnimation, ICollisionReciever
{
    [SerializeField] private AgentLinkMover agentMover;
    [SerializeField] private BodyFollowAgent bodyFollower;
    private bool isGrapplingPlayer = false;
    private bool waitingForGrapple = true;
    private bool canGrapplePlayer = true;
    private float distanceToPlayer = 999f;
    private Vector3 grapplePlayerOffset = Vector3.zero;
    private Vector3 startPosition = Vector3.zero;
    private SendFireSignal fireSignal;

    private void OnEnable()
    {
        Debug.Log("Ant enabled");
        this.startPosition = this.transform.position;
    }
    private void Start()
    {
        fireSignal = this.gameObject.GetComponentInChildren<SendFireSignal>();
        fireSignal.body = this;
    }
    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position);
        base.NotDyingUpdate();
    }
    public override void ReturnSelf()
    {
        agentMover.transform.localPosition = Vector3.zero;
        bodyFollower.transform.localPosition = Vector3.zero;
        isGrapplingPlayer = false;
        canGrapplePlayer = true;
        setHP(data.maxHP);
        StopAllCoroutines();
        this.gameObject.SetActive(false);
        parentPool.Return(this);
    }

    protected override void Attack()
    {
    }

    protected override void Die()
    {
        if (!isDying)
        {
            this.gameObject.SetActive(false);
            Debug.Log("ant died");
            animator.SetTrigger("Died");
            isDying = true;
        }
    }

    protected override void Move()
    {
        if (isGrapplingPlayer)
        {
            bodyFollower.transform.position = GameplayManager.Instance.Player.transform.position + grapplePlayerOffset;
            agentMover.transform.position = bodyFollower.transform.position;
        }
        else if (!canGrapplePlayer)
        {
            //TODO Should be a run away thing once fully implemented
            agentMover.SetDestination(startPosition);
        }
        else if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
        }
    }

    private IEnumerator Grapple(float grappleTime, float attackCooldown)
    {
        animator.SetTrigger("Fire1");
        isGrapplingPlayer = true;
        waitingForGrapple = true;
        canGrapplePlayer = false;
        grapplePlayerOffset = bodyFollower.transform.position - GameplayManager.Instance.Player.transform.position;
        yield return new WaitUntil(() => !waitingForGrapple);
        while (isGrapplingPlayer)
        {
            GameplayManager.Instance.Player.GetComponent<PlayerBody>().Slow(data.damage);
            yield return null;
        }
        RaycastHit hit;
        Physics.Raycast(bodyFollower.transform.position - GameplayManager.Instance.GetGravity(bodyFollower.transform.position).normalized, GameplayManager.Instance.GetGravity(bodyFollower.transform.position), out hit, 80, GameplayManager.Instance.NotPlayerOrEnemyMask, QueryTriggerInteraction.Ignore);
        if (hit.point != null)
        {
            agentMover.SetPosition(hit.point);
        }
        yield return new WaitForSeconds(attackCooldown);
        canGrapplePlayer = true;
    }

    public override void EndDeath()
    {
        ReturnSelf();
    }

    public void FireProjectile()
    {
        Debug.Log("Fire projectile");
        waitingForGrapple = false;
    }

    public void FireComplete()
    {
        isGrapplingPlayer = false;
    }

    public override void SetEnemyData(EnemyData data)
    {
        base.SetEnemyData(data);
        agentMover.agent.speed = data.moveSpeed * 2;
        bodyFollower.setSpeed(data.moveSpeed);
    }

    public void CollisionSignal(Collision collision)
    {

    }

    public void TriggerSignal(Collider collision)
    {
        if (canGrapplePlayer && collision.gameObject == GameplayManager.Instance.PlayerBody.gameObject)
        {
            StartCoroutine(Grapple(data.abilityCoolDown, data.attackCoolDown));
        }
    }
}
