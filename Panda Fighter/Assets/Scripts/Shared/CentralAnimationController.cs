using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAnimationController : MonoBehaviour
{
    protected CentralController controller;
    protected Animator animator;
    private Rigidbody2D rig;
    protected Camera camera;
    protected Transform body;

    public AnimationClip doubleJumpForwards;
    public AnimationClip doubleJumpBackwards;
    public Collider2D doubleJumpCollider;

    public bool carryOutDoubleJump;
    private bool isSpinning;

    public bool disableLimbsDuringDoubleJump { get; private set; }
    private bool completedSpinning;
    protected bool stopSpinning = true;
    protected int spinDirection = 0;

    private const float initialSpinSpeed = 1000f;
    private const float endSpinSpeed = 600f;
    private const float durationSpinningFast = 0.35f;

    private Vector2 initialColliderSize;

    private void Awake()
    {
        body = transform.GetChild(0);
        rig = transform.GetComponent<Rigidbody2D>();
        animator = body.GetComponent<Animator>();

        controller = transform.GetComponent<CentralController>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        initialColliderSize = controller.mainCollider.size;

        Side side = transform.parent.GetComponent<Role>().side;
        doubleJumpCollider.gameObject.layer = (side == Side.Friendly) ? Layers.friend : Layers.enemy;
        completedSpinning = true;
    }

    private void Update()
    {
        setAnimationState();
        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, controller.leftGroundChecker, controller.mainCollider));
    }

    private void FixedUpdate() 
    {
        if (carryOutDoubleJump)
            doDoubleJumpMidairSpin();

        // while spinning during a double jump, check when the creature becomes upright. Then, tell the creature to stop spinning
        if (isSpinning && (
                (spinDirection == -1 && transform.localEulerAngles.z < 30) || 
                (spinDirection == 1 && ((transform.localEulerAngles.z > 0 && transform.localEulerAngles.z < 40) || transform.localEulerAngles.z > 345))))
        {
            Debug.Log("x");
            completedSpinning = true;
            isSpinning = false;
        }
    }

    // Sets up the double jump animation. Specifies the direction to spin in, resets temporary spin settings,
    // disables concurrent limb updates (ex. can't control head movement while moving cursor) temporarily, 
    // and plays the actual double jump animation. 
    // Plays the forwards spin animation if the entity is facing the direction it's moving in.
    // Plays the backward spin animation if the entity is moving left but facing right or moving right but facing left. 
    public IEnumerator startDoubleJumpAnimation()
    {
        spinDirection = controller.dirX != 0 ? -controller.dirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;

        if (controller.dirX == -1 && body.localEulerAngles.y == 0)
            animator.SetBool("forward double jump", false);
        else if (controller.dirX == 1 && body.localEulerAngles.y == 180)
            animator.SetBool("forward double jump", false);
        else
            animator.SetBool("forward double jump", true);

        controller.mainCollider.enabled = false;
        doubleJumpCollider.enabled = true;
        completedSpinning = false;
        animator.SetInteger("jump version", 1);
        animator.SetBool("double jump", true);

        yield return new WaitForSeconds(0.5f);
        disableLimbsDuringDoubleJump = false;
    }

    // Specify which animation to play and when
    // Will enter running/walking animation once grounded and moving 
    // Will enter idle animation once grounded and not moving
    // Will enter jump animation when no longer grounded. 
    // Will enter the idle animation + reset jump for next time, once grounded after a jump
    // Note about PHASES below: 2 = jumping, 1 = walking, 0 = idle
    protected virtual void setAnimationState()
    {
        if (animator.GetInteger("Phase") == 2 && controller.isGrounded)
        {
            animator.SetBool("jumped", false);
            animator.SetInteger("jump version", 1);
            animator.SetInteger("Phase", (controller.dirX == 0) ? 0 : 1);
            animator.SetBool("double jump", false);
        }

        else if (controller.isGrounded)
            animator.SetInteger("Phase", (controller.dirX == 0) ? 0 : 1);
        
        /*else if (controller.isGrounded && controller.dirX == 0) // &&  (rig.velocity.y <= 0.1f || controller.isTouchingMap))
            animator.SetInteger("Phase", 0);*/

        else if (!controller.isGrounded)
            animator.SetInteger("Phase", 2);
    }

    // When double jumping, entity spins at a given initialSpinSpeed for a specified duration, 
    // then slows down to an endSpinSpeed. Note: entity's rotation is synced to the progress of 
    // the double jump spin animation. Updates when it's completed the double jump 
    private void doDoubleJumpMidairSpin()
    {
        if (!completedSpinning)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime  < 0.65f && !animator.IsInTransition(0))
            {
                float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

                if (t < durationSpinningFast) 
                    transform.eulerAngles = new Vector3(0, 0, t * initialSpinSpeed * spinDirection);
                else {
                    transform.eulerAngles = new Vector3(0, 0, 
                        durationSpinningFast * initialSpinSpeed * spinDirection + (t - durationSpinningFast) * endSpinSpeed * spinDirection);
                }

                if (!isSpinning && transform.localEulerAngles.z > 150 && transform.localEulerAngles.z < 250)
                    isSpinning = true;
            }
            else
                completedSpinning = true;
        }
        else if (animator.GetBool("double jump")) 
        {
            float z = (transform.localEulerAngles.z + 360) % 360;
            if (Mathf.Abs(z - 360) < 2|| Mathf.Abs(z) < 2)
            {
                doubleJumpCollider.enabled = false;
                controller.mainCollider.enabled = true;
                carryOutDoubleJump = false;
                isSpinning = false;
            }
            else 
            {
                z = (z < 180) ? transform.localEulerAngles.z - 2f : transform.localEulerAngles.z + 2f;
                transform.localEulerAngles = new Vector3(0, 0, z);
            }
        }
        else if (doubleJumpCollider.enabled) 
        {
            doubleJumpCollider.enabled = false;
            controller.mainCollider.enabled = true;
            carryOutDoubleJump = false;
            isSpinning = false; 
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

        mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.68f : 1f, mainCollider.size.y);
        mainCollider.size = new Vector2(mainCollider.size.x, animator.GetBool("double jump") && !stopSpinning ? initialColliderSize.y * 2f / 3f : initialColliderSize.y);
    }
}

