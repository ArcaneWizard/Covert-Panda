using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// A phase is the action the creature is doing right now (jumping, being idle, etc.)
// This class manages the phase the creature is in, and updates the creature's animations,
// colliders and feet position accordingly. Note: the animation controller
// is already setup to change animations according to the creature's phase

public class CentralPhaseTracker : MonoBehaviour
{
    public bool DisableLimbsDuringSomersault { get; private set; }

    protected CentralController controller;
    protected CentralLookAround lookAround;
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
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        Side side = transform.parent.GetComponent<Role>().side;

        doubleJumpCollider.gameObject.layer = (side == Side.Friendly) ? Layer.Friend : Layer.Enemy;
        doubleJumpCollider.enabled = false;
        controller.mainCollider.enabled = true;
        initialColliderSize = controller.mainCollider.size;

        somersaultHandler = new Somersault(transform, this, controller.mainCollider, doubleJumpCollider, 
            animator, controller);
        somersaultHandler.Reset();
    }

    void Update() 
    {
        if (health.isDead)
            return;

        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, 
            controller.leftGroundChecker, controller.mainCollider));
    }

    // returns whether or not the creature is in a specific phase 
    public bool Is(Phase specificPhase) => phase == (int)specificPhase;
    public bool IsMidAir => (2 <= phase && phase <= 4);

    public void EnterJumpPhase()
    {
        animator.SetInteger("jump version", UnityEngine.Random.Range(0, 2));
        setPhase(Phase.Jumping);
    }

    public IEnumerator EnterDoubleJumpPhase()
    {
        setPhase(Phase.DoubleJumping);

        DisableLimbsDuringSomersault = true;
        StartCoroutine(somersaultHandler.Begin());
        yield return new WaitForSeconds(0.5f);
        DisableLimbsDuringSomersault = false;
    }

    // get + set the creature's phase
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
            if ((controller.dirX == 1 && lookAround.IsLookingRight()) || controller.dirX == -1 && !lookAround.IsLookingRight())
                animator.SetBool("walking forwards", true);
            else if (controller.dirX != 0)
                animator.SetBool("walking forwards", false);
        }

        // creature does a somersault during double jumps
        somersaultHandler.Tick();
    }

    // Entity's feet, which detect ground, become closer together when jumping. Also, the main collider
    // becomes thinner when the entiy is jumping, and shorter when the entity is double jumping
    protected IEnumerator adjustFeetAndColliders(Transform rightFoot, Transform leftFoot, BoxCollider2D mainCollider) 
    {
        yield return new WaitForSeconds(Time.deltaTime);

        rightFoot.localPosition = (!IsMidAir)
        ? new Vector3(0.99f, rightFoot.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.localPosition.y, 0);

        leftFoot.localPosition = (!IsMidAir)
        ? new Vector3(-0.357f, leftFoot.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.localPosition.y, 0);

        float x = IsMidAir ? 0.68f : 1f;
        float y = (Is(Phase.DoubleJumping) && somersaultHandler.state != SomersaultState.Exited) 
            ? initialColliderSize.y * 2f / 3f 
            : initialColliderSize.y;
        mainCollider.size = new Vector2(x, y);
    }
}