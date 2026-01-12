using System.Collections;
using UnityEngine;

public class Ant : EnemyBase
{
    [SerializeField] private AgentLinkMover agentMover;
    [SerializeField] private BodyFollowAgent bodyFollower;
    private bool isGrapplingPlayer = false;
    private bool canGrapplePlayer = true;
    private float distanceToPlayer = 999f;
    private Vector3 grapplePlayerOffset = Vector3.zero;

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
        //TODO implement animator death
        ReturnSelf();
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
            agentMover.StopAgent();
        }
        else if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
        }
    }

    private IEnumerator Grapple(float grappleTime, float attackCooldown)
    {
        Debug.Log("Grapple");
        isGrapplingPlayer = true;
        canGrapplePlayer = false;
        grapplePlayerOffset = bodyFollower.transform.position - GameplayManager.Instance.Player.transform.position;
        yield return new WaitForSeconds(grappleTime);
        isGrapplingPlayer = false;
        yield return new WaitForSeconds(attackCooldown);
        canGrapplePlayer = true;
    }
}
