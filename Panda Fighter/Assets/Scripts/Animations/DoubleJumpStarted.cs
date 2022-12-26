using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpStarted : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        animator.transform.parent.GetComponent<CentralAnimationController>().StartDoubleJumpAnimation();
    }
}
