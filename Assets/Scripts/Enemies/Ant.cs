using System.Collections;
using UnityEngine;

public class Ant : EnemyBase, IFireAnimation
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
    }

    protected override void Attack()
    {
        if (canGrapplePlayer && distanceToPlayer <= data.attackRange)
        {
            StartCoroutine(Grapple(data.abilityCoolDown, data.attackCoolDown));
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
        Debug.Log("Grapple");
        animator.SetTrigger("Fire1");
        isGrapplingPlayer = true;
        waitingForGrapple = true;
        canGrapplePlayer = false;
        grapplePlayerOffset = bodyFollower.transform.position - GameplayManager.Instance.Player.transform.position;
        yield return new WaitUntil(() => !waitingForGrapple);
        //TODO slow player here
        yield return new WaitUntil(() => !isGrapplingPlayer);
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
        Debug.Log("Fire complete");
        isGrapplingPlayer = false;
    }

    public override void SetEnemyData(EnemyData data)
    {
        base.SetEnemyData(data);

    }
}
