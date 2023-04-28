using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIsOver : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.transform.GetComponent<CentralWeaponSystem>().CurrentWeaponBehaviour.ConfigureUponPullingOutWeapon();
    }
}
