using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// A phase is the action the creature is doing right now (jumping, being idle, etc.)
// This class manages the phase the creature is in, and updates the creature's animations,
// colliders and ground detectors' position accordingly. Note: the animation controller
// is already setup to change animations according to the creature's phase

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
    [SerializeField] private Collider2D doubleJumpCollider;

    void Awake() 
    {
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        controller = transform.GetComponent<CentralController>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        Side side = transform.parent.GetComponent<Role>().side;

        // colliders
        doubleJumpCollider.gameObject.layer = (side == Side.Friendly) ? Layer.Friend : Layer.Enemy;
        doubleJumpCollider.enabled = false;
        controller.mainCollider.enabled = true;

        // somersault handler
        somersaultHandler = new Somersault(transform, this, controller.mainCollider, doubleJumpCollider, 
            animator, controller);
        somersaultHandler.Reset();

        // setup animation clip overrides so different jump animations, falling animations, etc. can be used
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);
    }

    void Update() 
    {
        if (health.IsDead)
            return;

        adjustCollidersAndDetectors(controller.leftGroundChecker, controller.rightGroundChecker, 
            controller.mainCollider);
    }

    // returns whether or not the creature is in a specific phase 
    public bool Is(Phase specificPhase) => phase == (int)specificPhase;
    public bool IsMidAir => 2 <= phase && phase <= 4;
    public bool IsSomersaulting => Is(Phase.DoubleJumping) && somersaultHandler.state != SomersaultState.Exited;
    public bool IsWalkingBackwards => Is(Phase.Running) && !animator.GetBool("walking forwards");

    // set phase to jumping, and play any random one of the jump animations
    public void EnterJumpPhase()
    {
        // if creature is still, use the default idle jump animation. else use any
        if (controller.dirX == 0)
            clipOverrides["jumping"] = jumpClips[0];
        else
            clipOverrides["jumping"] = jumpClips[Random.Range(0, jumpClips.Length)];

        animatorOverrideController.ApplyOverrides(clipOverrides);
        setPhase(Phase.Jumping);
    }

    // set phase to double jumping, and execute a mid-air somersault
    public IEnumerator EnterDoubleJumpPhase()
    {
        setPhase(Phase.DoubleJumping);

        IsDoingSomersault = true;
        StartCoroutine(somersaultHandler.Begin());
        yield return new WaitForSeconds(0.31f);
        IsDoingSomersault = false;
    }

    private int phase => animator.GetInteger("Phase");
    private void setPhase(Phase p) => animator.SetInteger("Phase", (int)p);

    private void FixedUpdate() 
    {
        if (health.IsDead) 
        {
            somersaultHandler.Reset();
            return;
        }
        
        // the creature is idle or running if it's grounded and hasn't jumped recently
        if (controller.isGrounded && !controller.recentlyJumpedOffGround)
            setPhase((controller.dirX == 0) ? Phase.Idle : Phase.Running);

        // else the creature is falling if a mid-air phase (falling, jumping, double jumping) hasn't been set yet
        else if (!IsMidAir)
            setPhase(Phase.Falling);

        // play forward or backwards running animation depending on
        // whether the creature runs forwards or backwards 
        if (Is(Phase.Running))
        {
            if ((controller.dirX == 1 && lookAround.IsLookingRight()) 
                || controller.dirX == -1 && !lookAround.IsLookingRight())
                animator.SetBool("walking forwards", true);
            else if (controller.dirX != 0)
                animator.SetBool("walking forwards", false);
        }

        // creature does a somersault during double jumps
        somersaultHandler.Tick();
    }

    // Adjust creature's colliders and ground detectors as required 
    protected void adjustCollidersAndDetectors(Transform frontGroundRaycaster, 
        Transform backGroundRaycaster, BoxCollider2D mainCollider) 
    {
        // if creature is mid-air, bring ground raycasters closer to creature's centerline
        frontGroundRaycaster.localPosition = (!IsMidAir)
        ? new Vector3(-0.124f, frontGroundRaycaster.localPosition.y, 0)
        : new Vector3(-0.124f, frontGroundRaycaster.localPosition.y, 0);

        backGroundRaycaster.localPosition = (!IsMidAir)
        ? new Vector3(0.365f, backGroundRaycaster.localPosition.y, 0)
        : new Vector3(0.365f, backGroundRaycaster.localPosition.y, 0);

        // if creature is mid-air, main collider becomes thinner 
        float x = IsMidAir ? 0.68f : 1f;
        mainCollider.size = new Vector2(x, mainCollider.size.y);

        // creature's collider depends on it's current phase
        mainCollider.enabled = !IsSomersaulting;
        doubleJumpCollider.enabled = IsSomersaulting;
    }
}