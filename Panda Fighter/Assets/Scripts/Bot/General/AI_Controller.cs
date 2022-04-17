
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Controller : CentralController
{
    public Transform decisionZone { get; private set; }
    public AI_ACTION AI_action { get; private set; }
    private string action;
    public string actionProgress { get; private set; }

    private bool hasNotFallenYet;
    private bool leftPlatform;
    private int lastMovementDirX;
    private float randomXPos;
    private float randomSpeed;

    private float fallingDurationTimer;

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
        this.AI_action = AI_action;
        decisionZone = zone;
        actionProgress = "pending start";
    }

    // executes the initialized action and sets action progress to "in progress"
    // to execute the action, it updates the entity's speed and directions and calls on helper methods
    private void executeAction()
    {
        actionProgress = "in progress";

        speed = maxSpeed;
        dirX = AI_action.dirX;
        action = AI_action.action;

        if (action == "fallDown" || action == "fallDownCurve") 
            hasNotFallenYet = true;

        else if (action == "headStraight")
            actionProgress = "finished";

        else if (action == "normalJump" || action == "doubleJump" || action == "launchPad")
        {
            leftPlatform = false;
            StartCoroutine(executeVerticalMotionAction());
        }

        else
            Debug.LogError($"Action {action} has no hard coded AI logic.");
    }

    public override void Update()
    {
        // don't do anything if dead
        if (health.isDead) 
        {
            isTouchingMap = false;
            return;
        }

        base.Update();

        // if a fall action has been initiated, then execute it when the AI walks off a platform
        if (hasNotFallenYet && !isGrounded && !isTouchingMap) 
        {
            executeFall();
            hasNotFallenYet = false;
        }

        // if the AI is grounded on the map
        if (isGrounded)
        {
            // execute any action that has been setup and is pending start
            if (actionProgress == "pending start")
                executeAction();

            // if the AI has landed on a platform after falling, reset its speed and mark the action as finished
            else if (actionProgress == "in progress" && !hasNotFallenYet && (action == "fallDown" || action == "fallDownCurve"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // if the AI has landed on a platform after jumping, reset its speed and mark the action as finished
            else if (actionProgress == "in progress" && leftPlatform && (action == "normalJump" || action == "doubleJump" || action == "launchPad"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // AI always moves at its max speed when not carrying out an action
            if (actionProgress == "finished")
                speed = maxSpeed;
        }
        
        // if the AI executed a jump and is no longer on the ground, update that it left the platform
        if ((action == "normalJump" || action == "doubleJump" || action == "launchPad") && !isGrounded && !leftPlatform)
            leftPlatform = true;
        

        // If the action was falling down in a curve, then after a given duration, change the creature's speed midway during the descent/fall
        // Note, this can only occur when the creature is not grounded (still airborn), did fall off a platform, and the minimum time b4 changing speed
        // has elapsed (so as to prevent a bug where the creature's speed keeps on getting updated even after an unexpecedtly quick landing)
        if (fallingDurationTimer > 0)
            fallingDurationTimer -= Time.deltaTime;

        if (fallingDurationTimer <= 0 && !isGrounded && !hasNotFallenYet && action == "fallDownCurve")
        {
            randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
            dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
            speed = Mathf.Abs(randomSpeed);
        }
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private void executeFall()
    {
        // set fall speed only when actually falling
        if (AI_action.action == "fallDown")
            speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);

        // set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (AI_action.action == "fallDownCurve") 
        {
             lastMovementDirX = dirX;
             speed = AI_action.speed.x;
             fallingDurationTimer = UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y);
        }
    }

    private IEnumerator executeFallingDownCurveMotion()
    {
        lastMovementDirX = dirX;

        speed = AI_action.speed.x;
        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));

        randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
        hasNotFallenYet = false;
    }

    //------------------------------------------------------------------
    //---------Handle Jumping at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeVerticalMotionAction()
    {
        randomXPos = UnityEngine.Random.Range(AI_action.jumpBounds.x, AI_action.jumpBounds.y);
        dirX = Math.Sign(randomXPos - transform.position.x);

        while ((dirX == 1 && transform.position.x < randomXPos) || (dirX == -1 && transform.position.x > randomXPos)) 
            yield return null;

        dirX = AI_action.dirX;
        speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);

        if (AI_action.action == "launchPad")
            StartCoroutine(executeJumpPadLaunchAtRightMoment());

        else if (AI_action.action == "normalJump") 
        {
            if (isGrounded && !animator.GetBool("double jump"))
                normalJump();

            StartCoroutine(changeVelocityAfterDelay(AI_action.timeB4Change, AI_action.changedSpeed));
        }

        else if (AI_action.action == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment());
    }

    private IEnumerator executeDoubleJumpAtRightMoment()
    {
        
        if (isGrounded && !animator.GetBool("double jump"))
            normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        if (animator.GetBool("jumped") && !animator.GetBool("double jump"))
            doubleJump();

        StartCoroutine(changeVelocityAfterDelay(AI_action.timeB4SecondChange, AI_action.secondChangedSpeed));
    }

    //apply a huge jump pad boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeJumpPadLaunchAtRightMoment()
    {
        jumpPadBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
    }

    private void LateUpdate() => setAlienVelocity();

    // handles setting the alien velocity on slopes, while falling, etc.
    private void setAlienVelocity()
    {
        // nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        // when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && isGrounded && isTouchingMap)
        {
            rig.velocity = groundDir * speed * dirX;
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
        }

        // when alien is not on the ground, alien velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (dirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (dirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * dirX, rig.velocity.y);

            rig.gravityScale = maxGravity;
        }
    }

    // changes velocity after a given delay if you're still on the same action
    private IEnumerator changeVelocityAfterDelay(Vector2 delay, Vector2 velocity)
    {
        AI_ACTION action = AI_action;
        yield return new WaitForSeconds(UnityEngine.Random.Range(delay.x, delay.y));
        if (action.Equals(AI_action) && actionProgress == "in progress")
        {
            float randomSpeed = UnityEngine.Random.Range(velocity.x, velocity.y);
            dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
            speed = Mathf.Abs(randomSpeed);
        }
    }
}
