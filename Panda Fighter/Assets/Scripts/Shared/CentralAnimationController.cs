using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

// Manages when the creature goes to different animations. Updates the creature's colliders +
// feet position depending on the current animation 

public class CentralAnimationController : MonoBehaviour
{
    protected CentralController controller;
    protected Animator animator;
    private Health health;
    protected Camera camera;
    protected Transform body;
    public Collider2D doubleJumpCollider;

    public bool DisableLimbsDuringDoubleJump { get; private set; }
    private SomersaultState somersaultState;

    private const float initialSomersaultSpeed = 1000f;
    private const float endSomersaultSpeed = 600f;
    private const float somersaultDuration = 0.35f;
    protected int somersaultDirection = 0;

    private Vector2 initialColliderSize;

    private void Awake() 
    {
        body = transform.GetChild(0);
        animator = body.GetComponent<Animator>();
        controller = transform.GetComponent<CentralController>();
        health = transform.GetComponent<Health>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        Side side = transform.parent.GetComponent<Role>().side;
        doubleJumpCollider.gameObject.layer = (side == Side.Friendly) ? Layers.Friend : Layers.Enemy;

        initialColliderSize = controller.mainCollider.size;
        somersaultState = SomersaultState.Ended;
    }

    private void Update() 
    {
        if (health.isDead)
            return;

        updateAnimationState();
        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, 
            controller.leftGroundChecker, controller.mainCollider));
    }

    private void FixedUpdate() 
    {
        if (health.isDead) 
        {
            somersaultState = SomersaultState.Ended;
            return;
        }

        if (somersaultState == SomersaultState.HasStarted) 
        {
           executeDoubleJumpSomersault();

           if (transform.localEulerAngles.z > 150 && transform.localEulerAngles.z < 250)
                somersaultState = SomersaultState.Midway;
        }

        else if (somersaultState == SomersaultState.Midway) 
        {
            executeDoubleJumpSomersault();

            bool isCreatureUpright = false;
            if (somersaultDirection == -1)
                isCreatureUpright = transform.localEulerAngles.z < 30f;
            else if (somersaultDirection == 1) {
                isCreatureUpright = (transform.localEulerAngles.z > 0 && transform.localEulerAngles.z < 40) 
                    || transform.localEulerAngles.z > 345;
            }

            if (isCreatureUpright)
                somersaultState = SomersaultState.AlmostEnded;
        }

        else if (somersaultState == SomersaultState.AlmostEnded) 
        {
            endDoubleJumpSomersault();
            somersaultState = SomersaultState.Ended;
        }
    }

    public void StartDoubleJumpAnimation() => somersaultState = SomersaultState.HasStarted;

    // Specify which animation to play and when. 
    // Will enter idle animation once grounded + not moving
    // Will enter running animation once grounded + moving 
    // Will enter jump animation when no longer grounded. 
    // Note about animator's PHASE values below: 2 = jumping, 1 = walking, 0 = idle
    protected virtual void updateAnimationState() {
        if (controller.isGrounded) 
        {
            animator.SetBool("jumped", false);
            animator.SetInteger("Phase", (controller.dirX == 0) ? 0 : 1);
            animator.SetBool("double jump", false);
        }

        else
            animator.SetInteger("Phase", 2);
    }

    // Setup the double jump somersault. Specifies the direction to somersault in, disables concurrent limb updates temporarily
    // (ex. can't control head movement while moving cursor), and plays the mid-air somersault animation.
    // Temporarily disables creature's main collider and uses a different collider for the in-air summersault
    public IEnumerator SetupDoubleJumpSomersault()
    {
        somersaultDirection = controller.dirX != 0 ? -controller.dirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        DisableLimbsDuringDoubleJump = true;

        controller.mainCollider.enabled = false;
        doubleJumpCollider.enabled = true;

        // Plays the forwards spin animation if the entity is facing the direction it's moving in;
        // plays backward spin animation if the entity is facing one direction but moving in the other.
        animator.SetBool("double jump", true);
        if (controller.dirX == -1 && body.localEulerAngles.y == 0)
            animator.SetBool("forward double jump", false);
        else if (controller.dirX == 1 && body.localEulerAngles.y == 180)
            animator.SetBool("forward double jump", false);
        else
            animator.SetBool("forward double jump", true);


        yield return new WaitForSeconds(0.5f);
        DisableLimbsDuringDoubleJump = false;
    }

    // To double jump, the creature somersaults at a specified speed for a specified duration, 
    // then slows down rotating to a slower speed. The creature's rotation is synced to the progress of 
    // the double jump animation.
    private void executeDoubleJumpSomersault()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.65f && !animator.IsInTransition(0))
        {
            float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

            if (t < somersaultDuration)
                transform.eulerAngles = new Vector3(0, 0, t * initialSomersaultSpeed * somersaultDirection);
            else
            {
                transform.eulerAngles = new Vector3(0, 0,
                    somersaultDuration * initialSomersaultSpeed * somersaultDirection + (t - somersaultDuration) * endSomersaultSpeed * somersaultDirection);
            }
        }
        else
        {
            Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + "," + !animator.IsInTransition(0));
            somersaultState = SomersaultState.AlmostEnded;
        }
    }

    private void endDoubleJumpSomersault() 
    {
        if (animator.GetBool("double jump")) {
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
        else if (doubleJumpCollider.enabled) {
            doubleJumpCollider.enabled = false;
            controller.mainCollider.enabled = true;
        }
    }

    // Entity's feet, which detect ground, become closer together when jumping. Also, the main collider
    // becomes thinner when the entiy is jumping, and shorter when the entity is double jumping
    protected IEnumerator adjustFeetAndColliders(Transform rightFoot, Transform leftFoot, BoxCollider2D mainCollider) 
    {
        yield return new WaitForSeconds(0.03f);

        rightFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.99f, rightFoot.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.localPosition.y, 0);

        leftFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.357f, leftFoot.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.localPosition.y, 0);

        float x = animator.GetInteger("Phase") == 2 ? 0.68f : 1f;
        float y = (animator.GetBool("double jump") && somersaultState != SomersaultState.Ended) ? initialColliderSize.y * 2f / 3f : initialColliderSize.y;
        mainCollider.size = new Vector2(x, y);
    }

    private enum SomersaultState 
    {
        HasStarted, 
        Midway,
        AlmostEnded,
        Ended
    }
}

