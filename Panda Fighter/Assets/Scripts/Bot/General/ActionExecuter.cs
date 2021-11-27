using System;
using System.Collections;
using UnityEngine;

public class ActionExecuter
{
    private AI_Controller controls;
    private CentralAnimationController animControls;
    private Transform transform;
    private Animator animator;
    private Rigidbody2D rig;

    private float randomXPos;

    public ActionExecuter(AI_Controller controller, CentralAnimationController animControls,
           Animator animator, Rigidbody2D rig)
    {
        this.controls = controller;
        this.animControls = animControls;
        this.animator = animator;
        this.rig = rig;
        this.transform = controller.transform;
    }

    public IEnumerator headStraight(AI_ACTION AI_action)
    {
        randomXPos = UnityEngine.Random.Range(AI_action.bounds.x, AI_action.bounds.y);
        controls.setDirection(Math.Sign(randomXPos - transform.position.x));

        while ((controls.dirX == 1 && transform.position.x < randomXPos)
        || (controls.dirX == -1 && transform.position.x > randomXPos) || controls.actionProgress == "finished")
            yield return null;

        if (controls.actionProgress == "finished")
            yield break;

        controls.setDirection(AI_action.dirX);
        controls.setSpeed(CentralController.maxSpeed);
        controls.CurrentActionIsInProgress();
    }

    public bool executeFall(AI_ACTION AI_action, GameObject leftFootGround, GameObject rightFootGround)
    {
        //set fall speed only when actually falling
        if (!controls.isGrounded && !controls.isTouchingMap && AI_action.action == "fallDown")
        {
            controls.setSpeed(UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y));
            controls.CurrentActionIsInProgress();
            return false;
        }

        //set initial fall speed only when actually falling (+ will change dir midway during fall)
        else if (!controls.isGrounded && !controls.isTouchingMap && AI_action.action == "fallDownCurve")
        {
            controls.StartCoroutine(executeFallingDownCurveMotion(AI_action));
            controls.CurrentActionIsInProgress();
            return false;
        }

        //slow down right as the bot is about to fall (one foot off the ledge)
        if ((AI_action.action == "fallDownCurve" || AI_action.action == "fallDown") &&
            (!leftFootGround || !rightFootGround) && controls.isGrounded)
            controls.setSpeed(7f);

        return true;
    }

    private IEnumerator executeFallingDownCurveMotion(AI_ACTION AI_action)
    {
        controls.setSpeed(AI_action.speed.x);

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));

        float randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        controls.setDirection(AI_action.dirX * (int)Mathf.Sign(randomSpeed));
        controls.setSpeed(Mathf.Abs(randomSpeed));
    }

    public IEnumerator executeVerticalMotionAction(AI_ACTION AI_action)
    {
        randomXPos = UnityEngine.Random.Range(AI_action.bounds.x, AI_action.bounds.y);
        controls.setDirection(Math.Sign(randomXPos - transform.position.x));

        while ((controls.dirX == 1 && transform.position.x < randomXPos)
        || (controls.dirX == -1 && transform.position.x > randomXPos) || controls.actionProgress == "finished")
            yield return null;

        if (controls.actionProgress == "finished")
            yield break;

        controls.setDirection(AI_action.dirX);
        controls.setSpeed(UnityEngine.Random.Range(AI_action.speed.x, AI_action.speed.y));

        if (AI_action.action == "launchPad")
            controls.StartCoroutine(executeLaunchAtRightMoment(AI_action));

        else if (AI_action.action == "normalJump")
            controls.StartCoroutine(executeNormalJumpAtRightMoment());

        else if (AI_action.action == "doubleJump")
            controls.StartCoroutine(executeDoubleJumpAtRightMoment(AI_action));
    }

    private IEnumerator executeNormalJumpAtRightMoment()
    {
        normalJump();
        yield return new WaitForSeconds(Time.deltaTime * 2);
        controls.CurrentActionIsInProgress();
    }

    private IEnumerator executeDoubleJumpAtRightMoment(AI_ACTION AI_action)
    {
        normalJump();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        float randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        controls.setDirection(AI_action.dirX * (int)Mathf.Sign(randomSpeed));
        controls.setSpeed(Mathf.Abs(randomSpeed));

        doubleJump();
        controls.CurrentActionIsInProgress();
    }

    //apply a huge launch boost force and alter the alien's horizontal speed midway in its arc
    private IEnumerator executeLaunchAtRightMoment(AI_ACTION AI_action)
    {
        controls.launchBoost();

        yield return new WaitForSeconds(UnityEngine.Random.Range(AI_action.timeB4Change.x, AI_action.timeB4Change.y));
        float randomSpeed = UnityEngine.Random.Range(AI_action.changedSpeed.x, AI_action.changedSpeed.y);
        controls.setDirection(AI_action.dirX * (int)Mathf.Sign(randomSpeed));
        controls.setSpeed(Mathf.Abs(randomSpeed));

        yield return new WaitForSeconds(Time.deltaTime * 2);
        controls.CurrentActionIsInProgress();
    }

    private void normalJump()
    {
        if (controls.isGrounded && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, CentralController.jumpForce));
            animator.SetBool("jumped", true);
        }
    }

    private void doubleJump()
    {
        if (animator.GetBool("jumped") && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.gravityScale = CentralController.maxGravity;
            rig.AddForce(new Vector2(0, CentralController.doubleJumpForce));
            animControls.startDoubleJumpAnimation(controls.dirX, controls.leftFoot.gameObject, controls.rightFoot.gameObject);
        }
    }

}
