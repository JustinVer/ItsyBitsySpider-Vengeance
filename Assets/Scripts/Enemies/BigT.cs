using UnityEngine;

public class BigT : MonoBehaviour
{
    [SerializeField] private AgentLinkMover agentMover;
    [SerializeField] private BodyFollowAgent bodyFollower;
    private float distanceToPlayer = 999f;
    private SendFireSignal fireSignal;
    private State currentState = State.Summon;
    private bool trySummoning = false;

    private enum State
    {
        Summon,
        Jump,
        Attack
    }


    private void Update()
    {
        distanceToPlayer = Vector3.Distance(bodyFollower.transform.position, GameplayManager.Instance.Player.transform.position);
        switch (currentState)
        {
            case State.Summon:
                updateSummon();
                break;
            case State.Jump:
                updateJump();
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
            trySummoning = false;
        }
        else if (!trySummoning && !bodyFollower.Anim.GetCurrentAnimatorStateInfo(0).IsName("Summon"))
        {
            currentState = State.Attack;
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

    private void updateJump()
    {

    }

    private void updateAttack()
    {

    }
}
