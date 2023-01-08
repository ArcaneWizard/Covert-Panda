using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJumpAction : AIAction
{
    public DoubleJumpAction(AIActionType action, AIActionInfo info) : base(action, info) { }

    private bool leftPlatform;
    public override void StartExecution(AI_Controller controller, Transform transform)
    {
        leftPlatform = false;
        controller.StartCoroutine(doubleJump(transform));
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator doubleJump(Transform transform)
    {
        float randomXPos = UnityEngine.Random.Range(Info.JumpBounds.x, Info.JumpBounds.y);
        int dirX = Math.Sign(randomXPos - transform.position.x);

        while ((dirX == 1 && transform.position.x < randomXPos) || (dirX == -1 && transform.position.x > randomXPos))
            yield return null;

        if (!controller.IsActionBeingExecuted(this))
            yield break;

        controller.SetDirection(Info.DirX);
        controller.SetSpeed(getRandomSpeed(Info.Speed));

        if (controller.isGrounded && !controller.phaseTracker.Is(Phase.DoubleJumping))
            controller.NormalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.timeB4Change.x, currAction.timeB4Change.y));
        if (!stillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.changedSpeed);
        dirX = currAction.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
            doubleJump();

        StartCoroutine(changeVelocityAfterDelay(currAction.timeB4SecondChange, currAction.secondChangedSpeed, currAction));



        cospeed = getRandomReasonableSpeed(currAction.Speed);

        if (currAction.actionName == "launchPad")
            StartCoroutine(executeJumpPadLaunchAtRightMoment(currAction));

        else if (currAction.actionName == "normalJump")
        {
            if (isGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
                normalJump();

            StartCoroutine(changeVelocityAfterDelay(currAction.TimeB4Change, currAction.ChangedSpeed, currAction));
        }

        else if (currAction.actionName == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment(currAction));


    }
}
