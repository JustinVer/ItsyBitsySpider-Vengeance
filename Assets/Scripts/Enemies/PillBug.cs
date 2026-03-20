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
    [SerializeField] private float rotationSpeed = 60f;

    protected override void NotDyingUpdate()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.PlayerBody.transform.position);
        base.NotDyingUpdate();
    }
    public override void ReturnSelf()
    {
        agentMover.transform.localPosition = Vector3.zero;
        bodyFollower.transform.localPosition = Vector3.zero;
        setHP(data.maxHP);
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }


    protected override void Attack()
    {
        //Attacking is when the pill hits the player so no update needs to happen
    }

    public void CollisionSignal(Collision collision)
    {
        if (collision.gameObject == GameplayManager.Instance.Player)
        {
            playerHit(collision.gameObject.GetComponent<IDamageable>());
        }
    }

    public void playerHit(IDamageable player)
    {
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
            GameplayManager.Instance.PlayerBody.CurrentWebs++;
            animator.SetTrigger("Died");
            isDying = true;
        }
    }

    protected override void Move()
    {
        if (rollingPastPlayer)
        {
            agentMover.SetDestination(awayFromPlayerTarget);
            bodyFollower.transform.localRotation = Quaternion.Euler(rotationSpeed * Time.time, bodyFollower.transform.localRotation.y, bodyFollower.transform.localRotation.z);
            animator.SetBool("Moving", true);
        }
        else if (distanceToPlayer < data.detectionDistanceClose)
        {
            agentMover.SetDestination(GameplayManager.Instance.Player.transform.position);
            bodyFollower.transform.localRotation = Quaternion.Euler(rotationSpeed * Time.time, bodyFollower.transform.localRotation.y, bodyFollower.transform.localRotation.z);
            animator.SetBool("Moving", true);
        }
        else
        {
            agentMover.agent.velocity = Vector3.zero;
            bodyFollower.RB.linearVelocity = Vector3.zero;
            bodyFollower.RB.angularVelocity = Vector3.zero;
            //bodyFollower.transform.localRotation = Quaternion.Euler(0, 0, 0);
            animator.SetBool("Moving", false);
        }
        Debug.Log("Pill bug local rotation " + bodyFollower.transform.localRotation.eulerAngles + " " + rotationSpeed + " " + Time.deltaTime);
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
