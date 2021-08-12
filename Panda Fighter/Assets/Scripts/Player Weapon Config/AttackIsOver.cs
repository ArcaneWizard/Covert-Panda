using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIsOver : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        resetAttackAnimations(animator, animator.transform.GetComponent<CentralWeaponAttacks>());
    }

    public void resetAttackAnimations(Animator armAnimator, CentralWeaponAttacks weaponAttacks)
    {
        //turn off attack animation
        weaponAttacks.attackAnimationPlaying = false;

        //after throwing grenade
        if (armAnimator.GetInteger("Arms Phase") == 1)
            armAnimator.SetInteger("Arms Phase", 0);

        //after swinging scythe
        else if (armAnimator.GetInteger("Arms Phase") == 11)
            armAnimator.SetInteger("Arms Phase", 10);
    }
}
