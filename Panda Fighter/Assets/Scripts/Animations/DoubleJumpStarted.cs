using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpStarted : StateMachineBehaviour
{
    private CentralPhaseTracker phaseTracker;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) 
    {
        if (!phaseTracker)
            phaseTracker = animator.transform.parent.GetComponent<CentralPhaseTracker>();

       // phaseTracker.StartSomersault();
    }
}
