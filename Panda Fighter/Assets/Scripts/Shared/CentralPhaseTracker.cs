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
    }

    void Update() 
    {
        if (health.isDead)
            return;

        adjustCollidersAndDetectors(controller.leftGroundChecker, controller.rightGroundChecker, 
            controller.mainCollider);
    }

    // returns whether or not the creature is in a specific phase 
    public bool Is(Phase specificPhase) => phase == (int)specificPhase;
    public bool IsMidAir => 2 <= phase && phase <= 4;
    public bool IsSomersaulting => Is(Phase.DoubleJumping) && somersaultHandler.state != SomersaultState.Exited;

    public void EnterJumpPhase()
    {
        animator.SetInteger("jump version", Random.Range(0, 2));
        setPhase(Phase.Jumping);
    }

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
        if (health.isDead) 
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
            
        // if creature isn't jumping, reset the jump animation version 
        if (Is(Phase.Jumping))
            animator.SetInteger("jump version", 0);

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