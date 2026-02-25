using System.Collections;
using UnityEngine;

public class PillBug : EnemyBase, ICollisionReciever
{
    [SerializeField] private AgentLinkMover agentMover;
    [SerializeField] private BodyFollowAgent bodyFollower;
    private float distanceToPlayer = 999f;
    private bool rollingPastPlayer = false;
    private bool canAttack = true;
    private Vector3 awayFromPlayerTarget = Vector3.zero;
    Coroutine hitCoroutine;

    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position);
        base.NotDyingUpdate();
    }
    public override void ReturnSelf()
    {
        agentMover.transform.localPosition = Vector3.zero;
        bodyFollower.transform.localPosition = Vector3.zero;
        setHP(data.maxHP);
        StopAllCoroutines();
    }


    protected override void Attack()
    {
        //Attacking is when the pill hits the player so no update needs to happen
    }

    public void CollisionSignal(Collision collision)
    {
        Debug.Log("Reveied signal");
        if (collision.gameObject == GameplayManager.Instance.Player)
        {
            playerHit(collision.gameObject.GetComponent<IDamageable>());
        }
    }

    public void playerHit(IDamageable player)
    {
        Debug.Log("PlayerHitMethod " + canAttack + " " + player);
        if (canAttack && player != null)
        {
            if (hitCoroutine != null)
            {
                StopCoroutine(hitCoroutine);
            }
            hitCoroutine = StartCoroutine(HitPlayer(player));
        }
    }
    private IEnumerator HitPlayer(IDamageable player)
    {
        Debug.Log("Hit player coroutine");

        player.modifyHP(data.damage * -1);
        rollingPastPlayer = true;
        canAttack = false;
        RaycastHit hit;
        float angle = 60.0f;
        while ((!Physics.Raycast(bodyFollower.transform.position, Quaternion.AngleAxis(angle, -bodyFollower.transform.forward).eulerAngles, out hit, 80, GameplayManager.Instance.NotPlayerOrEnemyMask, QueryTriggerInteraction.Ignore) && angle > -10) || hit.point == null)
        {
            angle -= 1.0f;
        }
        awayFromPlayerTarget = hit.point;
        yield return new WaitForSeconds(data.attackCoolDown);
        canAttack = true;
        yield return new WaitForSeconds(data.abilityCoolDown - data.attackCoolDown);
        rollingPastPlayer = false;
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
        if (rollingPastPlayer)
        {
            //TODO Should be a run away thing once fully implemented
            agentMover.SetDestination(awayFromPlayerTarget);
            animator.SetBool("Moving", true);
        }
        else if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
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

    public override void EndDeath()
    {
        ReturnSelf();
    }
    public override void SetEnemyData(EnemyData data)
    {
        base.SetEnemyData(data);
        agentMover.agent.speed = data.moveSpeed * 2;
        bodyFollower.setSpeed(data.moveSpeed);
    }

    public void TriggerSignal(Collider collision)
    {
    }
}
