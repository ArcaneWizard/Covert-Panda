using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollingReset : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
        => animator.SetInteger("ragdolling", 2);
}
