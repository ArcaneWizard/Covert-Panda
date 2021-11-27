
using System;
using System.Collections;
using UnityEngine;

public class AI_Controller : CentralController
{
    public Transform decisionZone { get; private set; }
    public string actionProgress { get; private set; }
    public AI_ACTION AI_action { get; private set; }

    private ActionExecuter actionDecider;

    private bool needsToFall;
    private float randomXPos;
    private float randomSpeed;

    //action progress starts off finished
    public override void Start()
    {
        base.Start();
        actionDecider = new ActionExecuter(this, animController, animator, rig);

        dirX = 0;
        actionProgress = "finished";
    }

    //Define an action/decision, and set action progress to "pending start" 
    public void BeginAction(AI_ACTION action, Transform zone)
    {
        AI_action = action;
        decisionZone = zone;
        actionProgress = "pending start";
        needsToFall = false;
    }

    //The point in space in front of the AI 
    public Vector3 InFrontOfAI() => shootingArm.position + new Vector3(dirX, 0, 0);

    public void CurrentActionIsInProgress() => actionProgress = "in progress";
    public void CurrentActionIsFinished() => actionProgress = "finished";

    //excute the given action and set action progress to "in progress"
    private void executeAction()
    {
        String action = AI_action.action;

        if (action == "fallDown" || action == "fallDownCurve")
            needsToFall = true;

        else if (action == "headStraight")
            StartCoroutine(actionDecider.headStraight(AI_action));

        else if (action == "normalJump" || action == "doubleJump" || action == "launchPad")
            StartCoroutine(actionDecider.executeVerticalMotionAction(AI_action));

        else
            Debug.LogError($"Action {action} has no hard coded AI logic.");
    }

    public override void Update()
    {
        base.Update();
        DebugGUI.debugText3 = "speed: " + speed.ToString() + ", action progress: " + actionProgress;

        if (needsToFall)
            needsToFall = actionDecider.executeFall(AI_action, leftGround, rightGround);

        if (isGrounded && isTouchingMap)
        {
            if (actionProgress == "pending start")
            {
                speed = maxSpeed;
                dirX = AI_action.dirX;
                actionProgress = "started";
                executeAction();
            }

            // if an action has been executed, set action progress to 
            // finished once the AI lands back on a platform
            if (actionProgress == "in progress" && !needsToFall)
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            // AI moves at its max speed when not carrying out an action
            if (actionProgress == "finished")
                speed = maxSpeed;
        }
    }

    private void LateUpdate() => setAlienVelocity();

    private void setAlienVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when alien is on the ground, alien velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && isGrounded && isTouchingMap)
        {
            rig.velocity = groundDir * speed * dirX;
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
        }

        //when alien is not on the ground, alien velocity is just left/right with gravity applied
        else
        {
            rig.velocity = new Vector2(speed * dirX, rig.velocity.y);
            rig.gravityScale = maxGravity;
        }
    }

}
