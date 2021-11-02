
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Controller : CentralController
{
    public Transform decisionZone { get; private set; }
    public AI_ACTION AI_action { get; private set; }

    public bool needsToFall { get; private set; }
    public bool shouldExecuteAction { get; private set; }
    public string actionProgress { get; private set; }

    private int lastMovementDirX;
    private float randomDistance;
    private float randomSpeed;

    public override void Start()
    {
        base.Start();

        dirX = 0;
        actionProgress = "finished";
    }

    //------------------------------------------------------------------
    //-------Call the right AI action at the right time-----------------
    //------------------------------------------------------------------

    public void executeAction()
    {
        Debug.Log(AI_action.action + ", " + decisionZone.name);

        if (AI_action.action == "keepWalking")
            actionProgress = "in progress";

        else if (AI_action.action == "fallDown")
            needsToFall = true;

        else if (AI_action.action == "fallDownCurve")
            needsToFall = true;

        else if (AI_action.action == "normalJump")
            StartCoroutine(executeNormalJumpAtRightMoment());

        else if (AI_action.action == "doubleJump")
            StartCoroutine(executeDoubleJumpAtRightMoment());
    }

    private void Update()
    {
        tilt();

        if (needsToFall)
            executeFall();

        if (shouldExecuteAction && isGrounded && isTouchingMap)
        {
            speed = maxSpeed;
            dirX = AI_action.dirX;

            shouldExecuteAction = false;
            actionProgress = "started";
            executeAction();
        }

        if (actionProgress == "in progress" && isGrounded && isTouchingMap)
        {
            speed = maxSpeed * UnityEngine.Random.Range(0.95f, 1f);
            actionProgress = "finished";
        }
    }

    //------------------------------------------------------------------
    //----------Handle Falling at the right time------------------------
    //------------------------------------------------------------------

    private void executeFall()
    {
        //set fall speed only when actually falling
        if (needsToFall && !isGrounded && !isTouchingMap && AI_action.action == "fallDown")
        {
            speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);
            needsToFall = false;
            actionProgress = "in progress";
        }

        //set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (needsToFall && !isGrounded && !isTouchingMap && AI_action.action == "fallDownCurve")
        {
            StartCoroutine(executeFallingDownCurveMotion());
            needsToFall = false;
        }

        //slow down right as the bot is about to fall (one foot off the ledge)
        if (needsToFall && (AI_action.action == "fallDownCurve" || AI_action.action == "fallDown") && (!leftFootGround || !rightFootGround) && isGrounded)
            speed = 7f;
    }

    private IEnumerator executeFallingDownCurveMotion()
    {
        lastMovementDirX = dirX;

        speed = AI_action.speed.x;
        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));


        randomSpeed = AI_action.changedSpeed + UnityEngine.Random.Range(0, AI_action.bonusTrait.x);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
        actionProgress = "in progress";
    }

    //------------------------------------------------------------------
    //---------Handle Jumping at the right time------------------------
    //------------------------------------------------------------------

    private IEnumerator executeNormalJumpAtRightMoment()
    {
        dirX = (decisionZone.transform.position.x > transform.position.x) ? 1 : -1;

        randomDistance = UnityEngine.Random.Range(0.3f, 0.6f) * Mathf.Abs(Mathf.Cos(groundAngle));
        Debug.Log("randomDistance: " + randomDistance);

        while (dirX == 1 && transform.position.x - decisionZone.position.x < randomDistance)
            yield return null;
        while (dirX == -1 && transform.position.x - decisionZone.position.x > -randomDistance)
            yield return null;

        dirX = AI_action.dirX;
        speed = UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y);
        Debug.Log(speed + ", " + UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y));
        normalJump();

        yield return new WaitForSeconds(Time.deltaTime * 2);
        actionProgress = "in progress";
    }

    private IEnumerator executeDoubleJumpAtRightMoment()
    {
        dirX = (decisionZone.transform.position.x > transform.position.x) ? 1 : -1;

        randomDistance = UnityEngine.Random.Range(0.3f, 0.6f) * Mathf.Abs(Mathf.Cos(groundAngle));
        Debug.Log("randomDistance: " + randomDistance);

        while (
            (dirX == 1 && transform.position.x - decisionZone.position.x < randomDistance) ||
            (dirX == -1 && transform.position.x - decisionZone.position.x > -randomDistance)
        )
            yield return null;

        dirX = AI_action.dirX;
        speed = AI_action.speed.x;
        normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        randomSpeed = AI_action.changedSpeed + UnityEngine.Random.Range(0, AI_action.bonusTrait.x);
        dirX = AI_action.dirX * (int)Mathf.Sign(randomSpeed);
        speed = Mathf.Abs(randomSpeed);
        doubleJump();

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
            rig.AddForce(new Vector2(0, doublejumpForce));
            controller.startDoubleJumpAnimation(dirX, leftFoot.gameObject, rightFoot.gameObject);
        }
    }

    //-------------------------------------------------------------------------------------------
    //----------Functions used by other scripts to carry out intertwined logic-------------------
    //-------------------------------------------------------------------------------------------

    public void executeNewAIAction(AI_ACTION action)
    {
        AI_action = action;
        actionProgress = "started";
        shouldExecuteAction = true;
    }

    public void registerNewDecisionZone(Transform zone)
    {
        decisionZone = zone;
    }

    public void moveInOtherDirection()
    {
        dirX = (dirX != 1) ? 1 : -1;
    }

    //------------------------------------------------------------------
    //---------Handle Looking around and Moving------------------------
    //------------------------------------------------------------------

    private void LateUpdate()
    {
        lookAndAimInRightDirection();
        setAlienVelocity();
    }

    private void setAlienVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when alien is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && isGrounded && isTouchingMap)
        {
            rig.velocity = groundDir * speed * dirX;
            rig.gravityScale = (dirX == 0) ? 0f : maxGravity;
        }

        //when alien is not on the ground, player velocity is just left/right with gravity applied
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

            // rotateHeadAndWeapon(shootDirection, shootAngle, weaponAttacks.disableAiming);
        }
    }

}
