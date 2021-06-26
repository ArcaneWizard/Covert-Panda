using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAnimationController : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform bot;
    private Animator animator;

    public BoxCollider2D mainCollider;
    public BoxCollider2D footCollider;

    private Transform leftFoot;
    private Transform rightFoot;
    private bool decideMovementAfterFallingDown;

    private NewBotAI AI;
    private DecisionMaking decisionMaking;

    public bool stopSpinning = true;
    public bool disableSpinningLimbs = false;
    public int spinDirection = 0;
    public int spinRate = 420;

    // Start is called before the first frame update
    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        bot = transform.GetChild(0);
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        AI = transform.GetComponent<NewBotAI>();
        decisionMaking = transform.GetComponent<DecisionMaking>();

        leftFoot = AI.leftFoot;
        rightFoot = AI.rightFoot;
    }

    // Update is called once per frame
    void Update()
    {
        alienAnimationController();
        StartCoroutine(handleColliders());
    }

    void LateUpdate()
    {
        alienLimbsOrientation();
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("double jump"))
        {
            if (!stopSpinning || (transform.eulerAngles.z > 3 && transform.eulerAngles.z < 357))
                transform.eulerAngles = new Vector3(0, 0, (transform.eulerAngles.z + Time.deltaTime * spinRate * spinDirection));
        }
    }

    public IEnumerator timeDoubleSpin()
    {
        leftFoot.gameObject.SetActive(false);
        rightFoot.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.6f);
        stopSpinning = true;
        spinRate = 200;

        yield return new WaitForSeconds(0.1f);
        disableSpinningLimbs = false;
        leftFoot.gameObject.SetActive(true);
        rightFoot.gameObject.SetActive(true);
    }

    //handles alien orientation (left/right), gun rotation, gun position, head rotation
    private void alienLimbsOrientation()
    {
        //if alien isn't spinning in mid-air with a double jump
        if (!disableSpinningLimbs)
        {
            //alien faces left or right depending on mouse cursor
            if (rig.velocity.x > 0)
                bot.localRotation = Quaternion.Euler(0, 0, 0);
            else if (rig.velocity.x < 0)
                bot.localRotation = Quaternion.Euler(0, 180, 0);
        }
    }

    //states when to transition btwn diff alien animation states 
    private void alienAnimationController()
    {
        //if the alien isn't currently midair (ie. isn't in the jump animation state) 
        if (animator.GetInteger("Phase") != 2)
        {
            if (!AI.grounded)
                setAnimation("jumping");
            else if (AI.movementDirX != 0)
                setAnimation("walking");
            else
                setAnimation("idle");

            bool facingRight = (bot.localRotation.y == 0);

            //if you're looking in the opposite direction as you're running, set walking speed to -1 (which auto triggers backwards walking animation)
            if (animator.GetInteger("Phase") == 1)
            {
                if ((AI.movementDirX == 1 && facingRight) || AI.movementDirX == -1 && !facingRight)
                    animator.SetFloat("walking speed", 1);
                else if (AI.movementDirX != 0)
                    animator.SetFloat("walking speed", -1);
            }
        }

        //if alien is grounded, exit out of jump animation
        if (animator.GetInteger("Phase") == 2 && AI.grounded)
        {
            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);

            setAnimation("idle");
            decideMovementAfterFallingDown = true;

            AI.speed = 8.0f;
            rig.gravityScale = 1;
        }

        //after falling down and touching the map, decide on the new direction to head in
        if (decideMovementAfterFallingDown && AI.touchingMap)
        {
            decideMovementAfterFallingDown = false;
            StartCoroutine(decisionMaking.decideMovementAfterFallingDown());
        }
    }

    //foot collider becomes smaller when jumping
    private IEnumerator handleColliders()
    {
        yield return new WaitForSeconds(0.03f);

        //disable main foot's collider when not walking
        footCollider.gameObject.SetActive(animator.GetInteger("Phase") == 1);

        //tuck the feet ground raycasters in when jumping
        rightFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.597f, rightFoot.transform.localPosition.y, 0)
        : new Vector3(-0.002f, rightFoot.transform.localPosition.y, 0);

        leftFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.787f, leftFoot.transform.localPosition.y, 0)
        : new Vector3(-0.626f, leftFoot.transform.localPosition.y, 0);

        //thin collider when jumping
        //mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.7f : 1.035f, mainCollider.size.y);
    }


    //set new animation state for the alien
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

        //get animation progress (as a positive number btwn 0-1 regardless of whether the animation is played forwards or backwards)
        float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

        //if walking
        if (animator.GetInteger("Phase") == 1)
        {
            //go idle 
            if (newMode == 0 && ((t >= 0.31f && t <= 0.41f) || (t >= 0.8633f && t <= 0.975f)))
                StartCoroutine(walkingToIdle());

            //go jump
            if (newMode == 2 && ((t >= 0.31f && t <= 0.41f) || (t >= 0.773f && t <= 0.975f)))
                animator.SetInteger("Phase", 2);
        }

        else
            animator.SetInteger("Phase", newMode);
    }

    //Submethod that adds tiny delay before switching from walking to idle animation (cleaner transition from walking to other non-idle animations)
    private IEnumerator walkingToIdle()
    {
        yield return new WaitForSeconds(0.045f);

        if (AI.movementDirX != 0)
            yield return null;
        else
            animator.SetInteger("Phase", 0);
    }

}
