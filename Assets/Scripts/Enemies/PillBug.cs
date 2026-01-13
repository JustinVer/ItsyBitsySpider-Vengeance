using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
        NavMeshHit hit;
        float angle = 30.0f;
        do
        {
            NavMesh.Raycast(this.transform.position, Quaternion.AngleAxis(angle, this.transform.forward) * (this.transform.position + (this.transform.forward.normalized * 30)), out hit, NavMesh.AllAreas);
            angle -= 1.0f;
        } while (hit.position != null && angle > -10);

        awayFromPlayerTarget = hit.position;
        yield return new WaitForSeconds(data.attackCoolDown);
        canAttack = true;
        yield return new WaitForSeconds(data.abilityCoolDown - data.attackCoolDown);
        rollingPastPlayer = false;
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
        if (rollingPastPlayer)
        {
            //TODO Should be a run away thing once fully implemented
            agentMover.SetDestination(awayFromPlayerTarget);
        }
        else if (distanceToPlayer < data.detectionDistanceClose || (distanceToPlayer < data.detectionDistanceLineOfSight && Physics.Linecast(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position, GameplayManager.Instance.NotPlayerOrEnemyMask)))
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
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
}
