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

    public Camera camera;
    public Transform shootingArm;
    public Transform gun;
    public Transform head;

    private Transform leftFoot;
    private Transform rightFoot;
    private bool decideMovementAfterFallingDown;

    private NewBotAI AI;
    private DecisionMaking decisionMaking;

    [HideInInspector]
    public bool stopSpinning = true;
    [HideInInspector]
    public bool disableSpinningLimbs = false;
    [HideInInspector]
    public int spinDirection = 0;
    [HideInInspector]
    public int spinRate = 420;

    //ideal local gun coordinates when looking to the side, up or down 
    private Vector2 pointingRight = new Vector2(0.745f, 1.966f);
    private Vector2 pointingUp = new Vector2(-0.171f, 3.320f);
    private Vector2 pointingDown = new Vector2(-0.400f, 0.780f);
    private Vector2 shoulderPos = new Vector2(-0.420f, 2.150f);

    private float upVector, downVector, rightVector;
    private float up, right, down;

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

        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //ideal vector magnitudes from shoulder to specific gun coordinates
        upVector = (pointingUp - shoulderPos).magnitude;
        rightVector = (pointingRight - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
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
            if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
                bot.localRotation = Quaternion.Euler(0, 0, 0);
            else
                bot.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the alien is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (AI.alien.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            if (shootDirection.y >= 0)
            {
                float slope = (up - right) / 90f;
                float weaponRotation = shootAngle * slope + right;

                float dirSlope = (upVector - rightVector) / 90f;
                float weaponDirMagnitude = shootAngle * dirSlope + 1.271f;

                Vector2 gunLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
                gun.transform.localPosition = gunLocation;

                float headSlope = (122f - 92.4f) / 90f;
                head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
            }

            if (shootDirection.y < 0)
            {
                float slope = (down - right) / -90f;
                float weaponRotation = shootAngle * slope + right;

                float dirSlope = (downVector - rightVector) / -90f;
                float weaponDirMagnitude = shootAngle * dirSlope + 1.271f;

                Vector2 gunLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
                gun.transform.localPosition = gunLocation;

                float headSlope = (67f - 92.4f) / -90f;
                head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
            }
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
            if (!animator.GetBool("jumped"))
                decideMovementAfterFallingDown = true;

            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);
            setAnimation("idle");

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
