using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAnimationController : MonoBehaviour
{
    protected CentralController controller;
    protected Animator animator;
    protected AnimatorOverrideController animatorOverrideController;
    private Transform body;

    public AnimationClip doubleJumpForwards;
    public AnimationClip doubleJumpBackwards;

    public bool disableLimbsDuringDoubleJump { get; private set; }
    protected bool stopSpinning = true;
    protected int spinDirection = 0;

    private float initialSpinSpeed = 350f;
    private float endSpinSpeed = 11f;
    private float durationSpinningFast = 0.35f;

    private void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        body = transform.GetChild(0).transform;
        controller = transform.GetComponent<CentralController>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
    }

    private void Update()
    {
        setAnimationState();
        StartCoroutine(adjustFeetAndColliders(controller.rightFoot, controller.leftFoot, controller.mainCollider));
    }

    private void FixedUpdate() => carryOutDoubleJump();

    // Set up the double jump animation. Specify the direction to spin in, reset any temporary settings,
    // disable concurrent limb updates (ex. head movement while looking around), and start the actual 
    // double jump animation. When the entity is moving left but facing right (or moving right but
    // facing left), play the backwards spin animation. Otherwise play the forwards spin animation
    public IEnumerator startDoubleJumpAnimation()
    {
        spinDirection = controller.dirX != 0 ? -controller.dirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;
        animator.SetBool("double jump", true);

        if (controller.dirX == -1 && body.localEulerAngles.y == 0)
            animatorOverrideController["backwards double jump"] = doubleJumpBackwards;
        else if (controller.dirX == 1 && body.localEulerAngles.y == 180)
            animatorOverrideController["backwards double jump"] = doubleJumpBackwards;
        else
            animatorOverrideController["backwards double jump"] = doubleJumpForwards;

        yield return new WaitForSeconds(0.6f);
        Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
        disableLimbsDuringDoubleJump = false;
    }

    // Specify which animation to play (2 = jumping, 1 = walking, 0 = idle) and when
    protected virtual void setAnimationState()
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
            animator.SetBool("double jump", false);
            animator.SetInteger("Phase", 0);
        }
    }

    // When double jumping, entity spins
    // at a given initialSpinSpeed for a specified duration, then slows down to an endSpinSpeed.
    // Note: entity's rotation is synced to the progress of the double jump spin animation
    private void carryOutDoubleJump()
    {
        if (animator.GetBool("double jump"))
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
                animator.SetBool("double jump", false);
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
        mainCollider.size = new Vector2(mainCollider.size.x, animator.GetBool("double jump") && !stopSpinning ? 2f : 3.14f);
    }
}

