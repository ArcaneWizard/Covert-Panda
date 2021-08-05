
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AI_Controller : CentralController
{
    private TestingTrajectories trajectory;
    private AI_ACTION AI_action;
    private List<AI_ACTION> AI_ACTIONS = new List<AI_ACTION>();

    private bool needsToFall = false;

    public override void Start()
    {
        base.Start();
        movementDirX = -1;
    }

    private void Update()
    {
        //use A and D keys for left or right movement

        tilt();
        actionScenarios();
    }

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
        if (!animator.GetBool("jumped") && grounded && touchingMap)
            rig.velocity = groundDir * speed * movementDirX;

        //when alien is not on the ground, player velocity is just left/right with gravity applied
        else
            rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
    }


    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!controller.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on mouse cursor
            if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
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

            rotateHeadAndWeapon(shootDirection, shootAngle, weaponAttacks.disableAiming);
        }
    }

    private void normalJump()
    {
        if (grounded && !animator.GetBool("double jump"))
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
            rig.AddForce(new Vector2(0, doublejumpForce));
            controller.startDoubleJumpAnimation(movementDirX, leftFoot.gameObject, rightFoot.gameObject);
        }
    }

    //if the AI bot collides with a decision making zone
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            AI_ACTIONS.Clear();

            foreach (Transform decision in col.transform)
                addTrajectoryAsPossibleAction(decision);

            int r = UnityEngine.Random.Range(0, AI_ACTIONS.Count);
            AI_action = AI_ACTIONS[r];

            executeAction();
        }
    }

    private void addTrajectoryAsPossibleAction(Transform decision)
    {
        trajectory = decision.transform.GetComponent<TestingTrajectories>();

        if (trajectory.headStraight)
            AI_action = new AI_ACTION("keepWalking", trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, decision.GetChild(0).position);
        else if (trajectory.fallDown)
            AI_action = new AI_ACTION("fallDown", trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, decision.GetChild(0).position);
        else if (trajectory.fallDownCurve)
            AI_action = new AI_ACTION("fallDownCurve", trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, decision.GetChild(0).position);
        else if (trajectory.doubleJump)
            AI_action = new AI_ACTION("doubleJump", trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, decision.GetChild(0).position);
        else
            AI_action = new AI_ACTION("normalJump", trajectory.speedRange, trajectory.timeB4Change, trajectory.changedSpeed, decision.GetChild(0).position);

        AI_ACTIONS.Add(AI_action);
    }

    private void executeAction()
    {
        Debug.Log(AI_action.action);

        if (AI_action.action == "keep Walking")
            return;
        else if (AI_action.action == "fallDown")
            needsToFall = true;
        else if (AI_action.action == "fallDownCurve")
            needsToFall = true;
        else if (AI_action.action == "normalJump")
            StartCoroutine(normalJumpTimer());
        else if (AI_action.action == "doubleJump")
            StartCoroutine(doubleJumpTimer());
    }

    private void actionScenarios()
    {
        if (needsToFall && !grounded && !touchingMap && AI_action.action == "fallDown")
        {
            speed = UnityEngine.Random.Range(trajectory.speedRange.x, trajectory.speedRange.y);
            needsToFall = false;
        }

        else if (needsToFall && !grounded && !touchingMap && AI_action.action == "fallDownCurve")
        {
            StartCoroutine(fallDownCurve());
            needsToFall = false;
        }

        if (needsToFall && AI_action.action == "fallDownCurve" && (!leftFootGround || !rightFootGround) && grounded)
            speed = 7f;
    }

    private IEnumerator fallDownCurve()
    {
        int sign = movementDirX;
        Debug.Log(sign);

        speed = trajectory.speedRange.x;
        yield return new WaitForSeconds(UnityEngine.Random.Range(trajectory.timeB4Change.x, trajectory.timeB4Change.y));
        speed = trajectory.changedSpeed;
        movementDirX = sign * -1;
    }

    private IEnumerator normalJumpTimer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
        speed = UnityEngine.Random.Range(trajectory.speedRange.x, trajectory.speedRange.y);
        normalJump();
    }

    private IEnumerator doubleJumpTimer()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.2f));
        speed = trajectory.speedRange.x;
        normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(trajectory.timeB4Change.x, trajectory.timeB4Change.y));
        speed = trajectory.changedSpeed;
        doubleJump();
    }
}
