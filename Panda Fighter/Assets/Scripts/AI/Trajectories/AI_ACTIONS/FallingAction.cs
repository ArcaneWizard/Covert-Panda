using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingAction : AIAction
{
    public FallingAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool hasFallen;

    public override void Begin(AI_Controller controller)
    {
        base.Begin(controller);
        hasFallen = false;
    }

    public override void Run()
    {
        if (!controller.isGrounded && !hasFallen)
        {
            hasFallen = true;
            controller.StartCoroutine(fall());
        }

        if (controller.isGrounded && controller.isTouchingMap && hasFallen)
            Finished = true;
    }

    private IEnumerator fall()
    {
        float randomXPos = UnityEngine.Random.Range(Info.JumpBounds.x, Info.JumpBounds.y);
        int dirX = Math.Sign(randomXPos - creature.position.x);

        while ((dirX == 1 && creature.position.x < randomXPos) || (dirX == -1 && creature.position.x > randomXPos))
            yield return null;

        if (!controller.IsActionBeingExecuted(this))
            yield break;

        DirX = Info.DirX;
        Speed = getRandomSpeed(Info.Speed);

        ExecuteJumpBoostNow = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(Info.TimeB4Change.x, Info.TimeB4Change.y));

        if (!controller.IsActionBeingExecuted(this))
            yield break;

        float randomSpeed = getRandomSpeed(Info.ChangedSpeed);
        DirX = Info.DirX * (int)Mathf.Sign(randomSpeed);
        Speed = Mathf.Abs(randomSpeed);
    }
}
