using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpVersions : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        int jumpVersion = UnityEngine.Random.Range(1, 3);
        animator.SetInteger("jump version", jumpVersion);
    }
}
