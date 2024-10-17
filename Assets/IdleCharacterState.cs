using System.Linq;
using UnityEngine;

public class IdleCharacterState : StateMachineBehaviour
{
    public UltEvents.UltEvent EnteringState;
    public UltEvents.UltEvent ExitedState;

    //! Two scenarios:
    //! 1. Entering this state on statemachine enter, its the first state.
    //! 2. Entering this state with transitions from other states.
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"entered to state.");
        //EnteringState.Invoke();
        //animator.CrossFade("Root|Run", 0);

        //CrossFadeAfter(animator, (int)(200 * Time.timeScale));
        //var names = next.Select(clip => clip.clip.);
        //animator.CrossFade("Root|Run", 0.3f);

        //Log(animator, layerIndex);
    }

    private async void Log(Animator animator, int layerIndex)
    {
        await System.Threading.Tasks.Task.Delay(100);
        var next = animator.GetNextAnimatorStateInfo(layerIndex);
        Debug.Log($"{next.fullPathHash}");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"exited state");
        //ExitedState.Invoke();
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Debug.Log($"entered to state.{animator}");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}
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
