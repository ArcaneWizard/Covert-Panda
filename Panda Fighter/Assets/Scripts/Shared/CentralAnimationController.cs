using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralAnimationController : MonoBehaviour
{

    protected Animator animator;
    protected CentralController controller;
    protected Transform body;

    protected bool stopSpinning = true;
    [HideInInspector]
    public bool disableLimbsDuringDoubleJump = false;
    protected int spinDirection = 0;

    private void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        body = transform.GetChild(0).transform;
        controller = transform.GetComponent<CentralController>();
    }

    private void Update()
    {
        setAnimationState();
        StartCoroutine(adjustCollidersBasedOnState(controller.footCollider, controller.rightFoot, controller.leftFoot, controller.mainCollider));
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("double jump"))
        {
            float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;
            if (t > 0.9f)
                stopSpinning = true;

            if (!stopSpinning || t > 0.1f)
            {
                if (t < 0.5f)
                    transform.eulerAngles = new Vector3(0, 0, t * spinDirection * 350f / 0.5f);
                else
                    transform.eulerAngles = new Vector3(0, 0, 350 * spinDirection + (t - 0.5f) * spinDirection * 21);

            }
            else
                animator.SetBool("double jump", false);
        }
    }

    public void startDoubleJumpAnimation(int movementDirX, GameObject leftFoot, GameObject rightFoot)
    {
        spinDirection = movementDirX != 0 ? -movementDirX : ((body.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;

        StartCoroutine(timeDoubleSpin(leftFoot, rightFoot));
        animator.SetBool("double jump", true);
    }

    //states when to transition btwn diff player animation states 
    protected void setAnimationState()
    {
        //if the player isn't currently in the jump state 
        if (animator.GetInteger("Phase") != 2)
        {
            if (!controller.isGrounded)
                setAnimation("jumping");
            else if (controller.dirX != 0)
                setAnimation("walking");
            else
                setAnimation("idle");

            bool facingRight = Input.mousePosition.x >= controller.camera.WorldToScreenPoint(controller.shootingArm.parent.position).x;

            //if you're looking in the opposite direction as you're running, set walking speed to -1 (which auto triggers backwards walking animation)
            if (animator.GetInteger("Phase") == 1)
            {
                if ((controller.dirX == 1 && facingRight) || controller.dirX == -1 && !facingRight)
                    animator.SetFloat("walking speed", 1);
                else if (controller.dirX != 0)
                    animator.SetFloat("walking speed", -1);
            }
        }

        //if you are grounded, exit out of jump animation
        if (animator.GetInteger("Phase") == 2 && controller.isGrounded)
        {
            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);
            setAnimation("idle");
        }
    }

    //set new animation state for the player
    private void setAnimation(string mode)
    {
        int newMode = 0;

        if (mode == "idle")
            newMode = 0;
        else if (mode == "walking")
            newMode = 1;
        else if (mode == "jumping")
            newMode = 2;
        else
            Debug.LogError("mode not defined");

        animator.SetInteger("Phase", newMode);
    }


    private IEnumerator timeDoubleSpin(GameObject leftFoot, GameObject rightFoot)
    {
        leftFoot.SetActive(false);
        rightFoot.SetActive(false);

        yield return new WaitForSeconds(0.52f);
        leftFoot.SetActive(true);
        rightFoot.SetActive(true);

        yield return new WaitForSeconds(0.12f);
        disableLimbsDuringDoubleJump = false;
    }

    //foot collider becomes smaller when jumping
    protected IEnumerator adjustCollidersBasedOnState(BoxCollider2D footCollider, Transform rightFoot, Transform leftFoot, BoxCollider2D mainCollider)
    {
        yield return new WaitForSeconds(0.03f);

        //disable main foot's collider when not walking
        footCollider.gameObject.SetActive(animator.GetInteger("Phase") == 1);

        //tuck the feet ground raycasters in when jumping
        rightFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.99f, rightFoot.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.localPosition.y, 0);

        leftFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.357f, leftFoot.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.localPosition.y, 0);

        //thin collider when jumping
        mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.68f : 1f, mainCollider.size.y);

        //shorten collider when double jumping
        mainCollider.size = new Vector2(mainCollider.size.x, animator.GetBool("double jump") && !stopSpinning ? 2f : 3.14f);
    }
}

