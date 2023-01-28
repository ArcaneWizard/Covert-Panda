using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FallingAction : AIAction
{
    public FallingAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasFallen;

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);

        DirX = Info.DirX;
        Speed = CentralController.MaxSpeed;
        hasFallen = false;
    }

    public override void Run()
    {
        if (!controller.isGrounded && !hasFallen)
        {
            hasFallen = true;
            executeCoroutine(fall());
        }

        if (controller.isGrounded && controller.isTouchingMap && hasFallen)
            Finished = true;
    }

    private IEnumerator fall()
    {
        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        yield return new WaitForSeconds(UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y));

        float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);
    }
}
