using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// A phase is the "action" the creature is doing right now (jumping, being idle, etc.)
// Manages which phase the creature is in, and updates the creature's animations,
// colliders and feet position accordingly. Note: the animation controller
// is setup to change animations according to the creature's phase

// More phases can be added with sub phases. For example,
// if the creature's main phase is set to "mid-air", that indicates the 
// the creature is doing something in the air. The sub-phase midAirPhase
// could specify what action they're doing in the air.

public abstract class CentralPhaseManager : MonoBehaviour
{
    public bool DisableLimbsDuringDoubleJump { get; private set; }

    protected CentralController controller;
    protected Animator animator;
    private Health health;
    protected Camera camera;
    protected Transform body;
    private Somersault somersaultHandler;

    [SerializeField] private Collider2D doubleJumpCollider;
    private Vector2 initialColliderSize;

    void Awake() 
    {
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        controller = transform.GetComponent<CentralController>();
        health = transform.GetComponent<Health>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        Side side = transform.parent.GetComponent<Role>().side;

        doubleJumpCollider.gameObject.layer = (side == Side.Friendly) ? Layers.Friend : Layers.Enemy;
        doubleJumpCollider.enabled = false;
        controller.mainCollider.enabled = true;
        initialColliderSize = controller.mainCollider.size;

        somersaultHandler = new Somersault();
        somersaultHandler.Initialize();
    }

    void Update() 
    {
        if (health.isDead)
            return;

        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, 
            controller.leftGroundChecker, controller.mainCollider));
    }

    public int Phase => animator.GetInteger("Phase");
    public int MidAirPhase => animator.GetInteger("PhaseMidAir");

    public bool Jump() => SetPhaseMidAir(MidAirPhases.Jumping);

    public bool DoubleJump() 
    {
        SetPhaseMidAir(MidAirPhases.DoubleJumping);
        somersaultHandler.Start();
    }

    // returns whether the following condition is true
    public bool IsFalling => Phase == Phases.MidAir && MidAirPhase == MidAirPhases.Default;
    public bool IsJumping => Phase == Phases.MidAir && MidAirPhase == MidAirPhases.Jumping;
    public bool IsDoubleJumping => Phase == Phases.MidAir && MidAirPhase == MidAirPhases.DoubleJumping;

    private void FixedUpdate() 
    {
        if (health.isDead) 
        {
            somersaultHandler.Reset();
            return;
        }
        
        // correctly set the creature's phase (idle, running or mid-air)
        if (controller.isGrounded && !controller.recentlyJumpedOffGround)
            SetPhase((controller.dirX == 0) ? Phases.Idle : Phases.Running);
        else
            SetPhase(Phases.MidAir);

        // if creature isn't mid-air, the mid-air phase will reset to it's default
        if (Phase != Phases.MidAir)
            SetMidAirPhase(PhasesMidAir.Default);
            
        // if creature isn't jumping, reset the jump animation version 
        if (MidAirPhase != PhasesMidAir.Jumping)
            animator.SetInteger("jump version", 0);

        // play forward or backwards running animation depending on
        // whether the creature runs forwards or backwards 
        if (Phase == Phases.Running)
        {
            if ((controller.dirX == 1 && facingRight()) || controller.dirX == -1 && !facingRight())
                animator.SetBool("walking forwards", true);
            else if (controller.dirX != 0)
                animator.SetBool("walking forwards", false);
        }

        // creature does a somersault during double jumps
        somersaultHandler.Run();
    }

    // Entity's feet, which detect ground, become closer together when jumping. Also, the main collider
    // becomes thinner when the entiy is jumping, and shorter when the entity is double jumping
    protected IEnumerator adjustFeetAndColliders(Transform rightFoot, Transform leftFoot, BoxCollider2D mainCollider) 
    {
        yield return new WaitForSeconds(0.03f);

        rightFoot.localPosition = (Phase != Phases.MidAir)
        ? new Vector3(0.99f, rightFoot.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.localPosition.y, 0);

        leftFoot.localPosition = (Phase != Phases.MidAir)
        ? new Vector3(-0.357f, leftFoot.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.localPosition.y, 0);

        float x = (Phase == Phases.MidAir) ? 0.68f : 1f;
        float y = (MidAirPhase == PhasesMidAir.DoubleJumping && somersaultState != Somersault.Exited) 
            ? initialColliderSize.y * 2f / 3f 
            : initialColliderSize.y;
        mainCollider.size = new Vector2(x, y);
    }

    protected abstract bool facingRight();

    private void SetPhase(int p) => animator.SetInteger("Phase", p);
    private void SetMidAirPhase(int p) => animator.SetInteger("MidAirPhase", p);
}