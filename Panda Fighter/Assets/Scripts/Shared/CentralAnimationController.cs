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

    //cached variables
    private float colliderSize;
    private float jumpDelay;

    private void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        body = transform.GetChild(0).transform;
        controller = transform.GetComponent<CentralController>();

        AnimatorHandler = new AnimatorHandler(animator);

        proceduralAnimator = gameObject.AddComponent<ProceduralAnimator>();
        proceduralAnimator.PretendConstructor(AnimatorHandler, animator);
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

    public void startJumpAnimation()
    {
        animator.SetBool("jumped", true);
        jumpDelay = 0.3f;
    }

    public void startDoubleJumpAnimation()
    {
        animator.SetBool("double jump", true);
    }

    private void Update()
    {
        updateAnimatorLogic();
        proceduralAnimator.Tick();
    }

    private void updateAnimatorLogic()
    {
        if (jumpDelay > 0f)
        {
            jumpDelay -= Time.deltaTime;
            return;
        }

        if ((animator.GetBool("jumped") || animator.GetBool("double jump")) && controller.isGrounded)
        {
            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);
        }
    }
}

