using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// Manages which phase/state the creature is in. Note that the animation controller
// will select the proper animation based off the creature's phase. Updates the creature's 
// colliders and feet position appropriately for it's current phase

public abstract class CentralPhaseManager : MonoBehaviour
{
    public bool DisableLimbsDuringDoubleJump { get; private set; }

    protected CentralController controller;
    protected Animator animator;
    private Health health;
    protected Camera camera;
    protected Transform body;

    private bool recentlyJumped;

    [SerializeField] private Collider2D doubleJumpCollider;
    private Vector2 initialColliderSize;

    private Somersault somersaultState;
    private const float initSomersaultSpeed = 1000f;
    private const float endSomersaultSpeed = 600f;
    private const float somersaultDuration = 0.35f;
    protected int somersaultDirection = 0;

    private void Awake() 
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
        somersaultState = Somersault.Exited;


    }

    private void Update() 
    {
        if (health.isDead)
            return;

        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, 
            controller.leftGroundChecker, controller.mainCollider));
    }

    // main phase creature is in
    public int Phase => animator.GetInteger("Phase");
    private void SetPhase(int p) => animator.SetInteger("Phase", p);

    // specific mid-air phase creature is in 
    public int PhaseMidAir => animator.GetInteger("PhaseMidAir");
    public void SetPhaseMidAir(int p) => animator.SetInteger("PhaseMidAir", p);

    // returns whether the following condition is true
    public bool IsFalling => Phase == Phases.MidAir && PhaseMidAir == PhasesMidAir.Default;
    public bool IsJumping => Phase == Phases.MidAir && PhaseMidAir == PhasesMidAir.Jumping;
    public bool IsDoubleJumping => Phase == Phases.MidAir && PhaseMidAir == PhasesMidAir.DoubleJumping;

    private void FixedUpdate() 
    {
        if (health.isDead) 
        {
            somersaultState = Somersault.Exited;
            return;
        }
        
        // correctly set the creature's phase (idle, running or mid-air)
        if (controller.isGrounded && !controller.recentlyJumped)
            SetPhase((controller.dirX == 0) ? Phases.Idle : Phases.Running);
        else
            SetPhase(Phases.MidAir);

        // if creature isn't mid-air, the mid-air phase will reset to it's default
        if (Phase != Phases.MidAir)
            SetPhaseMidAir(PhasesMidAir.Default);
            
        // if creature isn't jumping, reset the jump animation version 
        if (PhaseMidAir != PhasesMidAir.Jumping)
            animator.SetInteger("jump version", 0);

        // if creature isn't double jumping, reset the somersault direction 
        if (PhaseMidAir != PhasesMidAir.DoubleJumping)
            animator.SetBool("somersault forwards", true);

        // play forward or backwards running animation depending on
        // whether the creature runs forwards or backwards 
        if (Phase == Phases.Running)
        {
            if ((controller.dirX == 1 && facingRight()) || controller.dirX == -1 && !facingRight())
                animator.SetBool("walking forwards", true);
            else if (controller.dirX != 0)
                animator.SetBool("walking forwards", false);
        }

        if (somersaultState == Somersault.Started) 
        {
           doSomersault();
           
           if (transform.localEulerAngles.z > 150 && transform.localEulerAngles.z < 250)
                somersaultState = Somersault.UpsideDown;
        }

        else if (somersaultState == Somersault.UpsideDown) 
        {
            doSomersault();

            bool spunBackUpright = (somersaultDirection == -1 && transform.localEulerAngles.z < 30) 
                || (somersaultDirection == 1 && (transform.localEulerAngles.z > 0 && transform.localEulerAngles.z < 40))
                || (somersaultDirection == 1 && transform.localEulerAngles.z > 345);

            if (spunBackUpright)
                somersaultState = Somersault.NearFinished;
        }

        else if (somersaultState == Somersault.NearFinished) 
        {
            endSomersault();

            if (PhaseMidAir != PhasesMidAir.DoubleJumping) 
            {
                somersaultState = Somersault.Exited;
                doubleJumpCollider.enabled = false;
                controller.mainCollider.enabled = true;
            }
        }
    }

   protected abstract bool facingRight();

   public void StartSomersault() => somersaultState = Somersault.Started;

    // Execute a double jump somersault. Factors in the direction to somersault in, 
    // disables limb movement during the somersault (ex. can't control head movement with cursor), 
    // and plays the mid-air somersault animation. Creature has a small collider during the somersault
    public IEnumerator DoDoubleJump()
    {
        somersaultDirection = controller.dirX != 0 ? -controller.dirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        DisableLimbsDuringDoubleJump = true;

        if (controller.dirX == -1 && body.localEulerAngles.y == 0)
            animator.SetBool("somersault forwards", false);
        else if (controller.dirX == 1 && body.localEulerAngles.y == 180)
            animator.SetBool("somersault forwards", false);
        else
            animator.SetBool("somersault forwards", true);

        controller.mainCollider.enabled = false;
        doubleJumpCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        DisableLimbsDuringDoubleJump = false;
    }

    // To double jump, the creature somersaults at a specified speed for a specified duration, 
    // then slows down rotating to a slower speed. The creature's rotation is synced to the progress of 
    // the double jump animation.
    private void doSomersault()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.65f && !animator.IsInTransition(0))
        {
            float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

            if (t < somersaultDuration)
                transform.eulerAngles = new Vector3(0, 0, t * initSomersaultSpeed * somersaultDirection);
            else
            {
                float zAngle = somersaultDuration * initSomersaultSpeed * somersaultDirection
                        + (t - somersaultDuration) * endSomersaultSpeed * somersaultDirection;
                transform.eulerAngles = new Vector3(0, 0, zAngle);
            }
        }
        else
            somersaultState = Somersault.NearFinished;
    }

    private void endSomersault() 
    {
        float z = (transform.localEulerAngles.z + 360) % 360;

        if (Mathf.Abs(z - 360) < 2 || Mathf.Abs(z) < 2) {
            doubleJumpCollider.enabled = false;
            controller.mainCollider.enabled = true;
        }
        else {
            z = (z < 180) ? transform.localEulerAngles.z - 2f : transform.localEulerAngles.z + 2f;
            transform.localEulerAngles = new Vector3(0, 0, z);
        }
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
        float y = (PhaseMidAir == PhasesMidAir.DoubleJumping && somersaultState != Somersault.Exited) 
            ? initialColliderSize.y * 2f / 3f 
            : initialColliderSize.y;
        mainCollider.size = new Vector2(x, y);
    }

    private enum Somersault
    {
        Started,
        UpsideDown,
        NearFinished,
        Exited
    }
}