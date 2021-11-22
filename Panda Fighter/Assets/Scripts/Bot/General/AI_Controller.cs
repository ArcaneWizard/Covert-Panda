
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Controller : CentralController
{
    public Transform decisionZone { get; private set; }
    public AI_ACTION AI_action { get; private set; }
    public string actionProgress { get; private set; }

    private bool needsToFall;
    private int lastMovementDirX;
    private float randomXPos;
    private float randomSpeed;

    //action progress starts off finished
    public override void Start()
    {
        base.Start();

        dirX = 0;
        actionProgress = "finished";
    }

    //Define an action/decision, and set action progress to "pending start" 
    public void BeginAction(AI_ACTION action, Transform zone)
    {
        AI_action = action;
        decisionZone = zone;
        actionProgress = "pending start";
    }

    //The point in space in front of the AI 
    public Vector3 InFrontOfAI() => shootingArm.position + new Vector3(dirX, 0, 0);

    //excute the given action and set action progress to "in progress"
    private void executeAction()
    {
        actionProgress = "in progress";
        String action = AI_action.action;

        if (action == "fallDown" || action == "fallDownCurve")
            needsToFall = true;

        else if (action == "headStraight")
            dirX = AI_action.dirX;

        else if (action == "normalJump" || action == "doubleJump" || action == "launchPad")
            StartCoroutine(executeVerticalMotionAction());

        else
            Debug.LogError($"Action {action} has no hard coded AI logic.");
    }

    //called every frame
    public override void Update()
    {
        base.Update();

        //if a fall action has been initiated, execute its logic/checks 
        if (needsToFall)
            executeFall();

        //if the AI is on some platform/ground 
        if (isGrounded && isTouchingMap)
        {
            //execute any pending action 
            if (actionProgress == "pending start")
            {
                speed = maxSpeed;
                dirX = AI_action.dirX;
                actionProgress = "started";
                executeAction();
            }

            //if an action has been executed, set action progress to 
            //"finished" once the AI lands back on a platform
            if (actionProgress == "in progress" && !needsToFall)
            {
                speed = maxSpeed;
                actionProgress = "finished";
            }

            //AI moves at its max speed when not carrying out an action
            if (actionProgress == "finished")
                speed = maxSpeed;
        }
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private void executeFall()
    {
        //set fall speed only when actually falling
        if (!isGrounded && !isTouchingMap && AI_action.action == "fallDown")
        {
            speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);
            needsToFall = false;
        }

        //set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (!isGrounded && !isTouchingMap && AI_action.action == "fallDownCurve")
        {
            StartCoroutine(executeFallingDownCurveMotion());
            needsToFall = false;
        }

        //slow down right as the bot is about to fall (one foot off the ledge)
        if ((AI_action.action == "fallDownCurve" || AI_action.action == "fallDown") && (!leftFootGround || !rightFootGround) && isGrounded)
            speed = 7f;
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
            controller.startDoubleJumpAnimation(dirX, leftFoot.gameObject, rightFoot.gameObject);
        }
    }

    //Handle looking around and Moving
    private void LateUpdate()
    {
        lookAndAimInRightDirection();
        setAlienVelocity();
    }

    //handles setting the alien velocity on slopes, while falling, etc.
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

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!controller.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on mouse cursor
            if (dirX >= 0)
                body.localRotation = Quaternion.Euler(0, 0, 0);
            else
                body.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and creature's shooting arm
            Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the creature is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;
        }
    }

}
