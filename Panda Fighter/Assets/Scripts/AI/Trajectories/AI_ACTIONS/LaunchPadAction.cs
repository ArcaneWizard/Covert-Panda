using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LaunchPadAction : AIAction
{
    public LaunchPadAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasJumped;

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);
        executeCoroutine(launchPadBoost());

        hasJumped = false;
    }

    public override void Run()
    {
        if (!controller.isGrounded && !hasJumped)
            hasJumped = true;

        if (controller.isGrounded && hasJumped)
            Finished = true;
    }

    private IEnumerator launchPadBoost()
    {
        float randomXPos = UnityEngine.Random.Range(Info.Bounds.x, Info.Bounds.y);
        DirX = Math.Sign(randomXPos - creature.position.x);
        Speed = CentralController.MaxSpeed;

        while ((DirX == 1 && creature.position.x < randomXPos) || (DirX == -1 && creature.position.x > randomXPos))
            yield return null;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        ExecuteJumpBoostNow = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y));

        float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);
    }
}
