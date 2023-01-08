
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : CentralController
{
    // the current action the AI is executing
    public AI_ACTION currAction { get; private set; }

    // the decision zone that provided the current action the AI should execute
    private Transform decisionZone;

    // the status of the current action the AI is executing
    public Status ActionProgress { get; private set; }

    private HashSet<Transform> decisionZonesCreatureIsTouching;
    private string actionName;
    
    private bool hasNotFallenYet;
    private bool leftPlatform;
    private float randomXPos;
    private float randomSpeed;

    protected override void Start()
    {
        base.Start();

        dirX = UnityEngine.Random.Range(0, 2) * 2 - 1;
        ActionProgress = Status.Ended;
        decisionZone = null;

        decisionZonesCreatureIsTouching= new HashSet<Transform>();
    }

    // Return the coordinates of a point in space in front of the AI's upper body
    public Vector3 InFrontOfAI() => shootingArm.position + new Vector3(dirX, 0, 0);

    // Begin executing a specified action.
    public void StartAction(AI_ACTION AI_action, Transform zone = null)
    {
        currAction = AI_action;
        decisionZone = zone;
        ActionProgress = Status.PendingStart;
    }

    // End the current action
    public void EndAction() => ActionProgress = Status.Ended;

    // Change the x direction of the creature's movement. This is still implemented using actions for consistency.
    public void ChangeDirection(int dir)
    {
        AI_ACTION changeDirections = new AI_ACTION(dir);
        StartAction(changeDirections);
    }

    void Update()
    {
        // don't do anything if dead
        if (health.isDead) 
        {
            isTouchingMap = false;
            EndAction();
            return;
        }

        // if a fall action has been initiated, then execute it when the AI walks off a platform
        if (hasNotFallenYet && !isGrounded && !isTouchingMap) 
        {
            if (currAction.actionName == "fallDown")
                speed = UnityEngine.Random.Range(currAction.speed.x, currAction.speed.y);

            else if (currAction.actionName == "fallDownCurve") 
                StartCoroutine(executeFallingDownCurveMotion(currAction));

            hasNotFallenYet = false;
        }

        if (isGrounded && isTouchingMap)
        {
            // if the AI's action is pending start, execute the action provided by the decision zone (but only if
            // the AI hasn't ventured far from that decision zone yet)
            if (ActionProgress == Status.PendingStart)
            {
                if (decisionZonesCreatureIsTouching.Contains(decisionZone) || currAction.actionName == "change dir")
                    executeCurrentAction();
                else
                    EndAction();
            }

            // if the AI has landed on a platform after falling, reset its speed and mark the action as finished
            else if (ActionProgress == Status.InProgress && !hasNotFallenYet && (actionName == "fallDown" || actionName == "fallDownCurve"))
            {
                speed = maxSpeed;
                ActionProgress = Status.Ended;
            }

            // if the AI has landed on a platform after jumping, reset its speed and mark the action as finished
            else if (ActionProgress == Status.InProgress && leftPlatform && (actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad"))
            {
                speed = maxSpeed;
                ActionProgress = Status.Ended;
            }

            // AI always moves at its max speed when not carrying out an action
            if (ActionProgress == Status.Ended) 
                speed = maxSpeed;
        }
        
        // if the AI executed a jump and is no longer on the ground, update that it left the platform
        if ((actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad") && !isGrounded && !leftPlatform)
            leftPlatform = true;
    }

    // executes the current action (ex. jumping, falling, etc.)
    private void executeCurrentAction()
    {
        ActionProgress = Status.InProgress;

        speed = maxSpeed;
        dirX = currAction.dirX;
        actionName = currAction.actionName;

        if (actionName == "fallDown" || actionName == "fallDownCurve")
            hasNotFallenYet = true;

        else if (actionName == "headStraight" || actionName == "setDir")
            ActionProgress = Status.Ended;

        else if (actionName == "normalJump" || actionName == "doubleJump" || actionName == "launchPad")
        {
            leftPlatform = false;
            StartCoroutine(executeVerticalMotionAction(currAction));
        }

        else
            Debug.LogError($"Action {actionName} has no hard coded AI logic.");
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeFallingDownCurveMotion(AI_ACTION action)
    {
        speed = getRandomReasonableSpeed(currAction.speed);
        
        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.timeB4Change.x, currAction.timeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.changedSpeed);
        dirX = currAction.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.timeB4SecondChange.x, currAction.timeB4SecondChange.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.secondChangedSpeed);
        dirX = currAction.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    //------------------------------------------------------------------
    //---------Handle Jumping at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeVerticalMotionAction(AI_ACTION action)
    {
        randomXPos = UnityEngine.Random.Range(currAction.jumpBounds.x, currAction.jumpBounds.y);
        dirX = Math.Sign(randomXPos - transform.position.x);

        while ((dirX == 1 && transform.position.x < randomXPos) || (dirX == -1 && transform.position.x > randomXPos)) 
            yield return null;
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        dirX = currAction.dirX;
        speed = getRandomReasonableSpeed(currAction.speed);

        if (currAction.actionName == "launchPad")
            StartCoroutine(executeJumpPadLaunchAtRightMoment(currAction));

        else if (currAction.actionName == "normalJump") 
        {
            if (isGrounded && !phaseTracker.Is(Phase.DoubleJumping) && !phaseTracker.Is(Phase.Jumping))
                normalJump();

            StartCoroutine(changeVelocityAfterDelay(currAction.timeB4Change, currAction.changedSpeed, currAction));
        }

        else if (currAction.actionName == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment(currAction));
    }

    private IEnumerator executeDoubleJumpAtRightMoment(AI_ACTION action)
    {
        if (isGrounded && !phaseTracker.Is(Phase.DoubleJumping))
            normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(currAction.timeB4Change.x, currAction.timeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(currAction.changedSpeed);
        dirX = currAction.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        if (phaseTracker.Is(Phase.Jumping) && !phaseTracker.Is(Phase.DoubleJumping))
            doubleJump();

        StartCoroutine(changeVelocityAfterDelay(currAction.timeB4SecondChange, currAction.secondChangedSpeed, currAction));
    }

    //apply a huge jump pad boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeJumpPadLaunchAtRightMoment(AI_ACTION action)
    {
        jumpPadBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(action.timeB4Change.x, action.timeB4Change.y));
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        randomSpeed = getRandomReasonableSpeed(action.changedSpeed);
        dirX = action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    protected override void LateUpdate() 
    {
        base.LateUpdate();
        setAlienVelocity();
    }

    // handles setting the alien velocity on slopes, while falling, etc.
    private void setAlienVelocity()
    {
        // nullify the slight bounce on a slope glitch when changing slopes
        if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        // when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!phaseTracker.IsMidAir && isGrounded && isTouchingMap)
        {
            if (ActionProgress == Status.Ended && wallToTheLeft) 
                dirX = 1;
            else if (ActionProgress == Status.Ended && wallToTheRight)
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
                ActionProgress = Status.Ended;
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
        if (!isCreatureStillExecutingThisAction(action))
            yield break;

        float randomSpeed = getRandomReasonableSpeed(velocity);
        dirX = currAction.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
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

    private bool isCreatureStillExecutingThisAction(AI_ACTION action) => currAction.Equals(action) && ActionProgress == Status.InProgress;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesCreatureIsTouching.Add(col.transform);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.DecisionZone)
            decisionZonesCreatureIsTouching.Remove(col.transform);
    }
}

public enum Status
{
    PendingStart,
    Started,
    InProgress,
    Ended
}
