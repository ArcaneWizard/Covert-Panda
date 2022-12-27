using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpStarted : StateMachineBehaviour
{
    private CentralPhaseManager phaseManager;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        if (!phaseManager)
            phaseManager = animator.transform.parent.GetComponent<CentralPhaseManager>();

        phaseManager.StartSomersault();
    }
}
