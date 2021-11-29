using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CentralAnimationController : MonoBehaviour
{

    protected Animator animator;
    protected CentralController controller;
    protected Transform body;

    public AnimatorHandler AnimatorHandler { private set; get; }
    protected ProceduralAnimator proceduralAnimator;
    public Still still;
    public Walking walking;
    public NotProcedural notProcedural;

    protected bool stopSpinning = true;
    [HideInInspector]
    public bool disableLimbsDuringDoubleJump = false;
    protected int spinDirection = 0;

    private float colliderSize;

    private void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        body = transform.GetChild(0).transform;
        controller = transform.GetComponent<CentralController>();

        AnimatorHandler = new AnimatorHandler(animator);

        proceduralAnimator = gameObject.AddComponent<ProceduralAnimator>();
        proceduralAnimator.PretendConstructor(AnimatorHandler);
        notProcedural = gameObject.AddComponent<NotProcedural>();
    }

    void Start()
    {
        alwaysTransition(null, Animation.jumping, () => !controller.isGrounded);
        alwaysTransition(walking, null, () => controller.dirX != 0);
        alwaysTransition(still, null, () => controller.dirX == 0);

        proceduralAnimator.SetAnimation(still, null);

        void alwaysTransition(ProceduralAnimation p, string m, Func<bool> condition)
        {
            if (p == null) p = notProcedural;
            proceduralAnimator.AddAlwaysCalledTransition(p, m, condition);
        }
    }

    private void Update()
    {
        updateAnimatorLogic();
        proceduralAnimator.Tick();
    }

    //states when to transition btwn diff player animation states 
    private void updateAnimatorLogic()
    {

        //if you are grounded, exit out of jump animation
        if (AnimatorHandler.IsPlaying(Animation.jumping) && controller.isGrounded)
            StartCoroutine(delayedJumpReset());
    }

    private IEnumerator delayedJumpReset()
    {
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("jumped", false);
        animator.SetBool("double jump", false);
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("double jump"))
        {
            float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;
            if (t > 0.9f)
                stopSpinning = true;

            if (!stopSpinning || t > 0.1f)
            {
                if (t < 0.5f)
                    transform.eulerAngles = new Vector3(0, 0, t * spinDirection * 350f / 0.5f);
                else
                    transform.eulerAngles = new Vector3(0, 0, 350 * spinDirection + (t - 0.5f) * spinDirection * 21);

            }
            else
                animator.SetBool("double jump", false);
        }
    }

    public void startDoubleJumpAnimation(int movementDirX, GameObject leftFoot, GameObject rightFoot)
    {
        spinDirection = movementDirX != 0 ? -movementDirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;

        StartCoroutine(timeDoubleSpin(leftFoot, rightFoot));
        animator.SetBool("double jump", true);
    }

    private IEnumerator timeDoubleSpin(GameObject leftFoot, GameObject rightFoot)
    {
        leftFoot.SetActive(false);
        rightFoot.SetActive(false);

        yield return new WaitForSeconds(0.52f);
        leftFoot.SetActive(true);
        rightFoot.SetActive(true);

        yield return new WaitForSeconds(0.12f);
        disableLimbsDuringDoubleJump = false;
    }

    //set new physical animation to play in the animator 
    private void setPhysicalAnimation(string mode)
    {
        int newMode = 0;

        if (mode == "idle")
            newMode = 0;
        else if (mode == "walking")
            newMode = 1;
        else if (mode == "jumping")
            newMode = 2;
        else
            Debug.LogError("mode not defined");

        animator.SetInteger("Phase", newMode);
    }
}

