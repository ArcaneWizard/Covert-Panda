using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Sideview_Controller controller;
    private Transform player;

    private bool stopSpinning = true;
    [HideInInspector]
    public bool disableLimbsDuringDoubleJump = false;
    private int spinDirection = 0;

    void Awake()
    {
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        player = transform.GetChild(0).transform;

        controller = transform.GetComponent<Sideview_Controller>();
    }

    void Update()
    {
        setAnimationState();
        StartCoroutine(adjustCollidersBasedOnState());
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

    public void startDoubleJumpAnimation()
    {
        spinDirection = (controller.movementDirX != 0) ? -controller.movementDirX : ((player.localEulerAngles.y == 0) ? -1 : 1);
        stopSpinning = false;
        disableLimbsDuringDoubleJump = true;

        StartCoroutine(timeDoubleSpin());
        animator.SetBool("double jump", true);
    }

    //states when to transition btwn diff player animation states 
    private void setAnimationState()
    {
        //if the player isn't currently in the jump state 
        if (animator.GetInteger("Phase") != 2)
        {
            if (!controller.grounded)
                setAnimation("jumping");
            else if (controller.movementDirX != 0)
                setAnimation("walking");
            else
                setAnimation("idle");

            bool facingRight = Input.mousePosition.x >= controller.camera.WorldToScreenPoint(controller.shootingArm.parent.position).x;

            //if you're looking in the opposite direction as you're running, set walking speed to -1 (which auto triggers backwards walking animation)
            if (animator.GetInteger("Phase") == 1)
            {
                if ((controller.movementDirX == 1 && facingRight) || controller.movementDirX == -1 && !facingRight)
                    animator.SetFloat("walking speed", 1);
                else if (controller.movementDirX != 0)
                    animator.SetFloat("walking speed", -1);
            }
        }

        //if you are grounded, exit out of jump animation
        if (animator.GetInteger("Phase") == 2 && controller.grounded)
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


    private IEnumerator timeDoubleSpin()
    {
        controller.leftFoot.gameObject.SetActive(false);
        controller.rightFoot.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.52f);
        controller.leftFoot.gameObject.SetActive(true);
        controller.rightFoot.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.12f);
        disableLimbsDuringDoubleJump = false;
    }

    //foot collider becomes smaller when jumping
    private IEnumerator adjustCollidersBasedOnState()
    {
        yield return new WaitForSeconds(0.03f);

        //disable main foot's collider when not walking
        controller.footCollider.gameObject.SetActive(animator.GetInteger("Phase") == 1);

        //tuck the feet ground raycasters in when jumping
        controller.rightFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.99f, controller.rightFoot.transform.localPosition.y, 0)
        : new Vector3(0.332f, controller.rightFoot.transform.localPosition.y, 0);

        controller.leftFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.357f, controller.leftFoot.transform.localPosition.y, 0)
        : new Vector3(-0.157f, controller.leftFoot.transform.localPosition.y, 0);

        //thin collider when jumping
        controller.mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.68f : 1f, controller.mainCollider.size.y);

        //shorten collider when double jumping
        controller.mainCollider.size = new Vector2(controller.mainCollider.size.x, animator.GetBool("double jump") && !stopSpinning ? 2f : 3.14f);
    }
}
