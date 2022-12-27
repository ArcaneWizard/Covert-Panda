
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : CentralController
{
    public Transform decisionZone { get; private set; }
    public AI_ACTION Action { get; private set; }
    private string actionName;
    public string actionProgress { get; private set; }

    private bool hasNotFallenYet;
    private bool leftPlatform;
    private float randomXPos;
    private float randomSpeed;

    // action progress starts off as "finished" so that a new action can be started
    public override void Start()
    {
        base.Start();

        dirX = UnityEngine.Random.Range(0, 2) * 2 - 1;
        actionProgress = "finished";
    }

    // return the coordinates of a point in space in front of the AI's upper body
    public Vector3 InFrontOfAI() => shootingArm.position + new Vector3(dirX, 0, 0);

    // Forcefully ends the current action so that a new action can hapoen
    public void ForcefullyEndCurrentAction() => actionProgress = "finished";

    // update the action to be carried out and set action progress to "pending start" 
    public void BeginAction(AI_ACTION AI_action, Transform zone)
    {
        this.Action = AI_action;
        decisionZone = zone;
        actionProgress = "pending start"; 
    }

    // executes the initialized action and sets action progress to "in progress"
    // to execute the action, it updates the entity's speed and directions and calls on helper methods
    private void executeAction()
    {
        actionProgress = "in progress";

        speed = maxSpeed;
        dirX = Action.dirX;
        actionName = Action.actionName;

        if (actionName == "fallDown" || actionName == "fallDownCurve") 
            hasNotFallenYet = true;

        else if (actionName == "headStraight")
            actionProgress = "finished";

        else if (actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad")
        {
            leftPlatform = false;
            StartCoroutine(executeVerticalMotionAction(Action));
        }

        else
            Debug.LogError($"Action {actionName} has no hard coded AI logic.");
    }

    void Update()
    {
        // don't do anything if dead
        if (health.isDead) 
        {
            isTouchingMap = false;
            return;
        }

        // if a fall action has been initiated, then execute it when the AI walks off a platform
        if (hasNotFallenYet && !isGrounded && !isTouchingMap) 
        {
            if (Action.actionName == "fallDown")
                speed = UnityEngine.Random.Range(Action.speed.x, Action.speed.y);

            else if (Action.actionName == "fallDownCurve") 
            {
                StartCoroutine(executeFallingDownCurveMotion(Action));
            }

            hasNotFallenYet = false;
        }

        // if the AI is grounded on the map
        if (isGrounded && isTouchingMap)
        {
            // execute any action that has been setup and is pending start
            if (actionProgress == "pending start")
                executeAction();

            // if the AI has landed on a platform after falling, reset its speed and mark the action as finished
            else if (actionProgress == "in progress" && !hasNotFallenYet && (actionName == "fallDown" || actionName == "fallDownCurve"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // if the AI has landed on a platform after jumping, reset its speed and mark the action as finished
            else if (actionProgress == "in progress" && leftPlatform && (actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // AI always moves at its max speed when not carrying out an action
            if (actionProgress == "finished") 
                speed = maxSpeed;
        }
        
        // if the AI executed a jump and is no longer on the ground, update that it left the platform
        if ((actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad") && !isGrounded && !leftPlatform)
            leftPlatform = true;
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private void executeFall()
    {
        // set fall speed only when actually falling
        if (Action.actionName == "fallDown")
            speed = UnityEngine.Random.Range(Action.speed.x, Action.speed.y);

        // set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (Action.actionName == "fallDownCurve") 
             StartCoroutine(executeFallingDownCurveMotion(Action));
    }

    private IEnumerator executeFallingDownCurveMotion(AI_ACTION currentAction)
    {
        speed = getRandomReasonableSpeed(Action.speed);
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(Action.timeB4Change.x, Action.timeB4Change.y));
        if (!currentAction.Equals(Action) || actionProgress != "in progress")
            yield break;

        randomSpeed = getRandomReasonableSpeed(Action.changedSpeed);
        dirX = Action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        yield return new WaitForSeconds(UnityEngine.Random.Range(Action.timeB4SecondChange.x, Action.timeB4SecondChange.y));
        if (!currentAction.Equals(Action) || actionProgress != "in progress")
            yield break;

        randomSpeed = getRandomReasonableSpeed(Action.secondChangedSpeed);
        dirX = Action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    //------------------------------------------------------------------
    //---------Handle Jumping at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeVerticalMotionAction(AI_ACTION currentAction)
    {
        randomXPos = UnityEngine.Random.Range(Action.jumpBounds.x, Action.jumpBounds.y);
        dirX = Math.Sign(randomXPos - transform.position.x);

        while ((dirX == 1 && transform.position.x < randomXPos) || (dirX == -1 && transform.position.x > randomXPos)) 
            yield return null;
        if (!currentAction.Equals(Action) || actionProgress != "in progress")
            yield break;

        dirX = Action.dirX;
        speed = getRandomReasonableSpeed(Action.speed);

        if (Action.actionName == "launchPad")
            StartCoroutine(executeJumpPadLaunchAtRightMoment(Action));

        else if (Action.actionName == "normalJump") 
        {
            if (isGrounded && !phaseManager.IsDoubleJumping && !phaseManager.IsJumping)
                normalJump();

            StartCoroutine(changeVelocityAfterDelay(Action.timeB4Change, Action.changedSpeed, Action));
        }

        else if (Action.actionName == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment(Action));
    }

    private IEnumerator executeDoubleJumpAtRightMoment(AI_ACTION currentAction)
    {
        if (isGrounded && !phaseManager.IsDoubleJumping)
            normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(Action.timeB4Change.x, Action.timeB4Change.y));
        if (!currentAction.Equals(Action) || actionProgress != "in progress")
            yield break;

        randomSpeed = getRandomReasonableSpeed(Action.changedSpeed);
        dirX = Action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        if (phaseManager.IsJumping && !phaseManager.IsDoubleJumping)
            doubleJump();

        StartCoroutine(changeVelocityAfterDelay(Action.timeB4SecondChange, Action.secondChangedSpeed, Action));
    }

    //apply a huge jump pad boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeJumpPadLaunchAtRightMoment(AI_ACTION action)
    {
        jumpPadBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(action.timeB4Change.x, action.timeB4Change.y));
        if (!action.Equals(this.Action) || actionProgress != "in progress")
            yield break;

        randomSpeed = getRandomReasonableSpeed(action.changedSpeed);
        dirX = action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    public override void LateUpdate() 
    {
        base.LateUpdate();
        setAlienVelocity();
    }

    // handles setting the alien velocity on slopes, while falling, etc.
    private void setAlienVelocity()
    {
        // nullify the slight bounce on a slope glitch when changing slopes
        if (!phaseManager.IsJumping && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        // when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!phaseManager.IsJumping && isGrounded && isTouchingMap)
        {
            if (actionProgress == "finished" && wallToTheLeft) 
                dirX = 1;
            else if (actionProgress == "finished" && wallToTheRight)
                dirX = -1;

            rig.velocity = groundSlope * speed * dirX;
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
        }

        // when alien is not on the ground (falling or midair after a jump)
        else 
        {
            // Stop moving horizontally if the AI is about to crash into a wall after a jump.
            if (rig.velocity.y > 0 && ((dirX == 1 && wallToTheRight) || (dirX == -1 && wallToTheLeft))) 
                rig.velocity = new Vector2(0, rig.velocity.y);

            // Change directions if the AI is falling left or right and about to crash into a wall.
            else if ((dirX == 1 && wallToTheRight) || (dirX == -1 && wallToTheLeft))
            {
                rig.velocity = new Vector2(0, rig.velocity.y);
                dirX = (int) -Mathf.Sign(dirX) * UnityEngine.Random.Range(0,2);
                speed = 22;
                actionProgress = "finished";
            }

            // Set velocity to the left/right with specified speed and direction. Vertical motion is affected by gravity
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);

            rig.gravityScale = maxGravity;
        }
    }

    // changes velocity after a given delay if you're still on the same action
    private IEnumerator changeVelocityAfterDelay(Vector2 delay, Vector2 velocity, AI_ACTION action)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(delay.x, delay.y));
        if (action.Equals(this.Action) && actionProgress == "in progress")
        {
            float randomSpeed = getRandomReasonableSpeed(velocity);
            dirX = this.Action.dirX * (int)Mathf.Sign(randomSpeed);
            speed = Mathf.Abs(randomSpeed);
        }
    }

    private float getRandomReasonableSpeed(Vector2 speedRange) 
    {
        float randomSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
        if (randomSpeed > -2.5f & randomSpeed < 2.5f)
            randomSpeed = 0f;
        else if (randomSpeed < 16f && randomSpeed > -16f)
            randomSpeed = 16f * Mathf.Sign(randomSpeed);
        
        return randomSpeed;
    }
}
