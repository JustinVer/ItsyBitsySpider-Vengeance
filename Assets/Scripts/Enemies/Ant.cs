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
    [SerializeField] private float slowPercentage = 0.5f;
    [SerializeField] private LegManager LegManager;

    private void OnEnable()
    {
        this.startPosition = this.transform.position;
    }
    private void Start()
    {
        fireSignal = this.gameObject.GetComponentInChildren<SendFireSignal>();
        fireSignal.body = this;
    }
    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.PlayerBody.transform.position);
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
        if (parentPool != null)
            parentPool.Return(this);
    }

    protected override void Attack()
    {
    }

    protected override void Die()
    {
        if (!isDying)
        {
            GameplayManager.Instance.PlayerBody.CurrentWebs++;
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
            animator.SetBool("Moving", true);
        }
        else if (distanceToPlayer < data.detectionDistanceLineOfSight && distanceToPlayer < data.detectionDistanceClose || Physics.Linecast(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask))
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
            animator.SetBool("Moving", true);
        }
        else
        {
            agentMover.agent.velocity = Vector3.zero;
            bodyFollower.RB.linearVelocity = Vector3.zero;
            bodyFollower.RB.angularVelocity = Vector3.zero;
            animator.SetBool("Moving", false);
        }
    }

    private IEnumerator Grapple(float grappleTime, float attackCooldown)
    {
        animator.SetTrigger("Fire1");
        LegManager.pauseLegs(true);
        isGrapplingPlayer = true;
        waitingForGrapple = true;
        canGrapplePlayer = false;
        grapplePlayerOffset = bodyFollower.transform.position - GameplayManager.Instance.Player.transform.position;
        yield return new WaitUntil(() => !waitingForGrapple);
        while (isGrapplingPlayer)
        {
            GameplayManager.Instance.Player.GetComponent<PlayerBody>().Slow(slowPercentage);
            yield return null;
        }
        LegManager.pauseLegs(false);
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
