using UnityEngine;

namespace APlusOrFail.Character.Sprites.Animations
{
    public class AutoReverseAnimation : StateMachineBehaviour
    {
        public string speedMultiplierName;
        private int speedMultiplierHash;

        private void OnEnable()
        {
            speedMultiplierHash = Animator.StringToHash(speedMultiplierName);
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.SetFloat(speedMultiplierHash, 1);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            if (stateInfo.normalizedTime >= 0.9)
            {
                animator.SetFloat(speedMultiplierHash, -1);
            }
            else if (stateInfo.normalizedTime <= 0.1)
            {
                animator.SetFloat(speedMultiplierHash, 1);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}
    }
}
