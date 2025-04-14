using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadAmmoStateForHelicopter : StateMachineBehaviour
{
    /* For shotgun or sniper rifle */
    public float reloadTime = 0.8f;//How much % of the reload animation is played
    private bool hasReload;//Determine whether it is currently reloading





    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasReload = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hasReload)
        {
            return;
        }

        if (stateInfo.normalizedTime >= reloadTime)
        {
            if (animator.GetComponent<Weapon_HelicopterGun>() != null)
            {
                animator.GetComponent<Weapon_HelicopterGun>().ShotGunReload();
            }

            hasReload = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasReload = false;
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
