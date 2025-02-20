using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// A phase is the action the creature is doing right now (jumping, being idle, etc.)
// This class manages the phase the creature is in, and updates the creature's animations,
// colliders and ground detectors' position accordingly. Note: the animation controller
// is already setup to change animations when you call the function setPhase(Phase p)

public class CentralPhaseTracker : MonoBehaviour
{
    public bool IsDoingSomersault { get; private set; }

    protected CentralController controller;
    protected CentralLookAround lookAround;
    protected Animator animator;
    private Health health;
    protected Camera camera;
    protected Transform body;

    private Somersault somersaultHandler;
    private AnimatorOverrideController animatorOverrideController;
    private AnimationClipOverrides clipOverrides;

    [SerializeField] private AnimationClip[] jumpClips;
    [SerializeField] private AnimationClip[] doubleJumpClips;
    [SerializeField] private Collider2D doubleJumpCollider;

    void Awake() 
    {
        // setup
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        controller = transform.GetComponent<CentralController>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        // somersault handler
        somersaultHandler = new Somersault(transform, this, animator, controller, lookAround);
        somersaultHandler.Reset();

        // setup animation clip overrides so different jump animation variations, falling animation variations, etc. can be used
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
    }

    // returns whether or not the creature is in a specific phase 
    public bool Is(Phase specificPhase) => phase == (int)specificPhase;

    // returns whether the creature is in an "air-born" phase (ex. jumping, falling, etc. as opposed to running)
    public bool IsMidAir => 2 <= phase && phase <= 4;

    // returns whether the creature is somersaulting
    public bool IsSomersaulting => Is(Phase.DoubleJumping) && somersaultHandler.state != SomersaultState.Exited;

    // returns whether the creature is walking backwards
    public bool IsWalkingBackwards => Is(Phase.Running) && !animator.GetBool("walking forwards");

    // Set the phase to jumping, and play any random variaton of the jump animation in the animator
    public void Jump()
    {
        // note: if creature is still, always use the default idle jump animation.
        if (controller.DirX == 0)
            clipOverrides["jumping"] = jumpClips[0];
        else
            clipOverrides["jumping"] = jumpClips[Random.Range(0, jumpClips.Length)];

        animatorOverrideController.ApplyOverrides(clipOverrides);
        setPhase(Phase.Jumping);
    }

    // Set the phase to double jumping, and execute a mid-air somersault
    public IEnumerator DoubleJump()
    {
        setPhase(Phase.DoubleJumping);

        IsDoingSomersault = true;
        StartCoroutine(somersaultHandler.Begin());
        yield return new WaitForSeconds(0.31f);
        IsDoingSomersault = false;
    }

    // Specify which somersault animation should be used in the animator, based on somersault direction
    public void SwapSomersaultAnimation(bool isForwardSomersault)
    {
        if (isForwardSomersault)
            clipOverrides["double jump"] = doubleJumpClips[0];
        else
            clipOverrides["double jump"] = doubleJumpClips[1];

        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    private void setPhase(Phase p) => animator.SetInteger("Phase", (int)p);
    private int phase => animator.GetInteger("Phase");

    private void FixedUpdate() 
    {
        if (health.IsDead) 
        {
            somersaultHandler.Reset();
            return;
        }

        // update the phase when the creature is idle or running 
        if (controller.isGrounded && !controller.isOnSuperSteepSlope && !controller.recentlyJumpedOffGround
            && somersaultHandler.state == SomersaultState.Exited)
            setPhase((controller.DirX == 0) ? Phase.Idle : Phase.Running);

        // update the phase when the creature is falling
        else if (!IsMidAir)
            setPhase(Phase.Falling);

        // play forward or backwards running animation depending on
        // whether the creature runs forwards or backwards 
        if (Is(Phase.Running))
        {
            if ((controller.DirX == 1 && lookAround.IsFacingRight) 
                || controller.DirX == -1 && !lookAround.IsFacingRight)
                animator.SetBool("walking forwards", true);
            else if (controller.DirX != 0)
                animator.SetBool("walking forwards", false);
        }

        // creature does a somersault during double jumps
        somersaultHandler.Tick();
    }
}
