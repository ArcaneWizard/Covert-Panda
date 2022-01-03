
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

    private bool needsToFall;
    private bool leftPlatform;
    private int lastMovementDirX;
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
            needsToFall = true;

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
        base.Update();

        // if a fall action has been initiated, execute its logic/checks 
        if (needsToFall)
            executeFall();

        // if the AI is on some platform/ground 
        if (isGrounded && isTouchingMap)
        {
            //execute any pending action 
            if (actionProgress == "pending start")
                executeAction();

            // if the action has landed on a platform after falling, reset its speed 
            if (actionProgress == "in progress" && !needsToFall && (action == "fallDown" || action == "fallDownCurve"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // if the action has landed on a platform after jumping, reset its speed 
            else if (actionProgress == "in progress" && leftPlatform && (action == "normalJump" || action == "doubleJump" || action == "launchPad"))
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // AI moves at its max speed when not carrying out an action
            if (actionProgress == "finished")
                speed = maxSpeed;
        }

        if ((action == "normalJump" || action == "doubleJump" || action == "launchPad") && !isGrounded && !leftPlatform)
            leftPlatform = true;
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private void executeFall()
    {
        // set fall speed only when actually falling
        if (!isGrounded && AI_action.action == "fallDown")
        {
            speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);
            needsToFall = false;
        }

        // set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (!isGrounded && AI_action.action == "fallDownCurve")
        {
            StartCoroutine(executeFallingDownCurveMotion());
            needsToFall = false;
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
            StartCoroutine(executeLaunchAtRightMoment());

        else if (AI_action.action == "normalJump")
            StartCoroutine(executeNormalJumpAtRightMoment());

        else if (AI_action.action == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment());
    }

    private IEnumerator executeNormalJumpAtRightMoment()
    {
        normalJump();
        yield return new WaitForSeconds(Time.deltaTime * 2);
        actionProgress = "in progress";

        StartCoroutine(changeVelocityAfterDelay(AI_action.timeB4Change, AI_action.changedSpeed));
    }

    private IEnumerator executeDoubleJumpAtRightMoment()
    {
        normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        doubleJump(); 
        actionProgress = "in progress";
        
        StartCoroutine(changeVelocityAfterDelay(AI_action.timeB4SecondChange, AI_action.secondChangedSpeed));
    }

    //apply a huge launch boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeLaunchAtRightMoment()
    {
        launchBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);

        yield return new WaitForSeconds(Time.deltaTime * 2);
        actionProgress = "in progress";
    }

    private void normalJump()
    {
        if (isGrounded && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }
    }

    private void doubleJump()
    {
        if (animator.GetBool("jumped") && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.gravityScale = maxGravity;
            rig.AddForce(new Vector2(0, doubleJumpForce));
            StartCoroutine(controller.startDoubleJumpAnimation());
        }
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
            Debug.Log("yeet");
            float randomSpeed = UnityEngine.Random.Range(velocity.x, velocity.y);
            dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
            speed = Mathf.Abs(randomSpeed);
        }
    }
}
