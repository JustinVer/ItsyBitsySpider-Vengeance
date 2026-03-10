using UnityEngine;

/// <summary>
/// Sends death signals from an animator to a parent script
/// </summary>
public class DeathTriggers : StateMachineBehaviour
{
    IDeathAnimation script = null;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Find the parent script to send the triggers to
        int i = 0;
        GameObject gameObject = animator.gameObject;
        script = animator.gameObject.GetComponent<IDeathAnimation>();
        while (script == null && i < 2)
        {
            gameObject = gameObject.transform.parent.gameObject;
            script = gameObject.GetComponent<IDeathAnimation>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //End death on the object
        if (script != null)
        {
            script.EndDeath();
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
