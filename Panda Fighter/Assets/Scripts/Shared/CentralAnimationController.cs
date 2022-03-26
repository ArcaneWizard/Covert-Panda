using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAnimationController : MonoBehaviour
{
    protected CentralController controller;
    protected Animator animator;
    protected Camera camera;
    protected Transform body;

    public AnimationClip doubleJumpForwards;
    public AnimationClip doubleJumpBackwards;

    public bool disableLimbsDuringDoubleJump { get; private set; }
    private bool completedDoubleJump;
    protected bool stopSpinning = true;
    protected int spinDirection = 0;
    private float initialSpinSpeed = 350f;
    private float endSpinSpeed = 11f;
    private float durationSpinningFast = 0.35f;

    private Vector2 initialColliderSize;

    private void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        body = transform.GetChild(0);
        controller = transform.GetComponent<CentralController>();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;

        initialColliderSize = controller.mainCollider.size;
    }

    private void Update()
    {
        setAnimationState();
        StartCoroutine(adjustFeetAndColliders(controller.rightGroundChecker, controller.leftGroundChecker, controller.mainCollider));
    }

    private void FixedUpdate() => carryOutDoubleJump();

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

        animator.SetInteger("jump version", 1);
        animator.SetBool("double jump", true);
        completedDoubleJump = false;

        yield return new WaitForSeconds(0.65f);
        disableLimbsDuringDoubleJump = false;
    }

    // Specify which animation to play (2 = jumping, 1 = walking, 0 = idle) and when.
    // Will enter jump animation (Phase 2) when no longer grounded. 
    // Will enter running/backwards walking animation (Phase 1) when moving 
    // Will enter idle animation (Phase 0) when not moving and not in jump animation
    // Will enter idle animation once grounded and last in jump animation
    /*protected virtual void setAnimationState()
    {
        if (animator.GetInteger("Phase") != 2)
        {
            if (!controller.isGrounded)
                animator.SetInteger("Phase", 2);
            else if (controller.dirX != 0)
                animator.SetInteger("Phase", 1);
            else
                animator.SetInteger("Phase", 0);
        }

        if (animator.GetInteger("Phase") == 2 && controller.isGrounded)
        {
            animator.SetBool("jumped", false);
            animator.SetInteger("jump version", 1);
            animator.SetBool("double jump", false);
            animator.SetInteger("Phase", 0);
        }
    }*/

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
            animator.SetBool("double jump", false);
            completedDoubleJump = true;
            animator.SetInteger("Phase", 0);
        }

        if (controller.isGrounded && controller.dirX != 0)
            animator.SetInteger("Phase", 1);

        else if (controller.isGrounded && controller.dirX == 0)
            animator.SetInteger("Phase", 0);

        else if (!controller.isGrounded)
            animator.SetInteger("Phase", 2);
    }

    // When double jumping, entity spins at a given initialSpinSpeed for a specified duration, 
    // then slows down to an endSpinSpeed. Note: entity's rotation is synced to the progress of 
    // the double jump spin animation. Updates when it's completed the double jump 
    private void carryOutDoubleJump()
    {
        if (!completedDoubleJump)
        {
            float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;
            if (t > 0.9f) stopSpinning = true;

            if (!stopSpinning || t > 0.1f)
            {
                if (t < durationSpinningFast)
                    transform.eulerAngles = new Vector3(0, 0, t * initialSpinSpeed / durationSpinningFast * spinDirection);
                else
                    transform.eulerAngles = new Vector3(0, 0, (initialSpinSpeed + (t - durationSpinningFast) * endSpinSpeed)) * spinDirection;
            }
            else
                completedDoubleJump = true;
        }
        else if (animator.GetBool("double jump"))
            transform.localEulerAngles = new Vector3(0, 0, 0);
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

