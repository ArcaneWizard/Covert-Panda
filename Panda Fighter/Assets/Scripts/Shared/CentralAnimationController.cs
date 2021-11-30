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
    public ProceduralAnimator proceduralAnimator { protected set; get; }
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
        To(null, Animation.doubleJumping, () => animator.GetBool("double jump"));
        To(null, Animation.jumping, () => animator.GetBool("jumped"));
        To(walking, null, () => controller.dirX != 0 && controller.isGrounded);
        To(still, null, () => controller.dirX == 0 && controller.isGrounded);

        proceduralAnimator.SetAnimation(still, null);

        void To(ProceduralAnimation p, string m, Func<bool> condition)
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

    private void updateAnimatorLogic()
    {
        //if you've jumped or double jumped, wait 0.3 seconds before starting checks if you're grounded again
        if ((animator.GetBool("jumped") || animator.GetBool("double jump")))
            StartCoroutine(delayedJumpReset());
    }

    private IEnumerator delayedJumpReset()
    {
        yield return new WaitForSeconds(0.3f);

        if (controller.isGrounded)
        {
            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);
        }
    }

    private void FixedUpdate()
    {
        /*if (animator.GetBool("double jump"))
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
        }*/
    }

    public void startDoubleJumpAnimation(int movementDirX, GameObject leftFoot, GameObject rightFoot)
    {
        /*spinDirection = movementDirX != 0 ? -movementDirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;

        StartCoroutine(timeDoubleSpin(leftFoot, rightFoot));*/
        animator.SetBool("double jump", true);
    }

    /*private IEnumerator timeDoubleSpin(GameObject leftFoot, GameObject rightFoot)
    {
        leftFoot.SetActive(false);
        rightFoot.SetActive(false);

        yield return new WaitForSeconds(0.52f);
        leftFoot.SetActive(true);
        rightFoot.SetActive(true);

        yield return new WaitForSeconds(0.12f);
        disableLimbsDuringDoubleJump = false;
    }*/
}

