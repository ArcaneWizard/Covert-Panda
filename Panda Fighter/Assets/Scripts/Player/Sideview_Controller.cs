
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Sideview_Controller : MonoBehaviour
{
    private Rigidbody2D rig;
    private Transform player;
    private Animator animator;

    public Transform shootingArm;
    public Transform head;
    public BoxCollider2D mainCollider;
    public Transform gun;
    public BoxCollider2D footCollider;
    public string nextToWall;

    public Camera camera;
    public Transform alienToFollow;

    private Vector3 cameraOffset;
    private float cameraPosX;
    private float cameraPosY;
    private float cameraVelocityX = 0.0f;
    private float cameraVelocityY = 0.0f;
    private float smoothTimeX = 0.15f;
    private float smoothTimeY = 0.4f;
    private float mouseDistance;
    public Transform centerOfMap;

    private float speed = 8.0f;
    private float jumpForce = 600;

    private bool stopSpinning = true;
    private bool disableLimbs = false;
    private int spinDirection = 0;
    private int spinRate = 420;

    public Transform leftFoot;
    public Transform rightFoot;
    public GameObject leftFootGround;
    public GameObject rightFootGround;
    public GameObject generalGround;

    [SerializeField]
    private bool grounded;
    [SerializeField]
    private bool touchingMap;
    [SerializeField]
    private float groundAngle;
    private Vector2 groundDir;

    private int movementDirX;
    private float zAngle;

    private float time = 0;
    private float frames = 0;

    //ideal local gun coordinates when looking to the side, up or down 
    private Vector2 pointingRight = new Vector2(0.642f, 0.491f);
    private Vector2 pointingUp = new Vector2(-0.24f, 1.68f);
    private Vector2 pointingDown = new Vector2(-0.407f, -0.675f);
    private Vector2 shoulderPos = new Vector2(-0.608f, 0.662f);

    private float upVector, downVector, rightVector;
    private float up, right, down;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        player = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        cameraOffset = camera.transform.position - transform.position;

        //ideal angle from shoulder to specific gun coordinates
        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //ideal vector magnitudes from shoulder to specific gun coordinates
        upVector = (pointingUp - shoulderPos).magnitude;
        rightVector = (pointingRight - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
    }

    void Update()
    {
        grounded = isGrounded();

        playerAnimationController();
        StartCoroutine(handleColliders());
        cameraMovement();

        //use A and D keys for left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W) && animator.GetBool("jumped") == true && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce * 1.3f));
            rig.gravityScale = 1.4f;

            spinDirection = (movementDirX != 0) ? -movementDirX : ((player.localEulerAngles.y == 0) ? -1 : 1);
            spinRate = 420;
            stopSpinning = false;
            disableLimbs = true;

            StartCoroutine(timeDoubleSpin());
            animator.SetBool("double jump", true);
        }

        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }

        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        setPlayerVelocity();
        tilt();
    }

    private void LateUpdate()
    {
        playerLimbsOrientation();
    }

    private void cameraMovement()
    {
        mouseDistance = (Input.mousePosition.x - (float)Screen.width / 2f) / (float)Screen.width;

        cameraPosX = Mathf.SmoothDamp(camera.transform.position.x, transform.position.x + mouseDistance * 10f, ref cameraVelocityX, smoothTimeX) + cameraOffset.x;
        cameraPosY = Mathf.SmoothDamp(camera.transform.position.y, transform.position.y + mouseDistance * 8f, ref cameraVelocityY, smoothTimeY) + cameraOffset.y;

        camera.transform.position = new Vector3(cameraPosX, cameraPosY, camera.transform.position.z);
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        if (!animator.GetBool("jumped") && rig.velocity.y > 0)
            rig.velocity = new Vector2(0, 0);

        //when player is on the ground, player velocity is parallel to the slanted ground 
        if (!animator.GetBool("jumped") && grounded && touchingMap)
        {
            //no x velocity when running into a wall to avoid bounce/fall glitch
            if (movementDirX == 1 && ((player.localEulerAngles.y == 0 && nextToWall == "Forward") || (player.localEulerAngles.y == 180 && nextToWall == "Backward")))
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (movementDirX == -1 && ((player.localEulerAngles.y == 0 && nextToWall == "Backward") || (player.localEulerAngles.y == 180 && nextToWall == "Forward")))
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * movementDirX;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (movementDirX == 1 && ((player.localEulerAngles.y == 0 && nextToWall == "Forward") || (player.localEulerAngles.y == 180 && nextToWall == "Backward")))
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (movementDirX == -1 && ((player.localEulerAngles.y == 0 && nextToWall == "Backward") || (player.localEulerAngles.y == 180 && nextToWall == "Forward")))
                rig.velocity = new Vector2(0, rig.velocity.y);

            //player velocity is just left or right (with gravity pulling the player down)
            else
                rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);
        }
    }

    //player's body should tilt slightly on the slanted platform
    private void tilt()
    {
        zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle = zAngle - 360;

        if (grounded)
        {
            float newGroundAngle = ((groundAngle - 360) / 1.4f);

            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f && groundAngle <= 180)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (groundAngle / 1.4f - zAngle) * 20 * Time.deltaTime);
            else if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20f * Time.deltaTime);
        }
        else if (!grounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !animator.GetBool("double jump"))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void playerLimbsOrientation()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!disableLimbs)
        {
            //player faces left or right depending on mouse cursor
            if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
                player.localRotation = Quaternion.Euler(0, 0, 0);
            else
                player.localRotation = Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the player is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (player.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
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

    //states when to transition btwn diff player animation states 
    private void playerAnimationController()
    {
        //if the player isn't currently in the jump state 
        if (animator.GetInteger("Phase") != 2)
        {
            if (!grounded)
                setAnimation("jumping");
            else if (movementDirX != 0)
                setAnimation("walking");
            else
                setAnimation("idle");

            bool facingRight = Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x;

            //if you're looking in the opposite direction as you're running, set walking speed to -1 (which auto triggers backwards walking animation)
            if (animator.GetInteger("Phase") == 1)
            {
                if ((movementDirX == 1 && facingRight) || movementDirX == -1 && !facingRight)
                    animator.SetFloat("walking speed", 1);
                else if (movementDirX != 0)
                    animator.SetFloat("walking speed", -1);
            }
        }

        //if you are grounded, exit out of jump animation
        if (animator.GetInteger("Phase") == 2 && grounded)
        {
            animator.SetBool("jumped", false);
            animator.SetBool("double jump", false);
            rig.gravityScale = 1;
            setAnimation("idle");
        }
    }

    //check if the player is on the ground + update the groundAngle
    public bool isGrounded()
    {
        GameObject collider = null;

        if (leftFootGround != null && rightFootGround == null)
            collider = leftFootGround;
        else if (rightFootGround != null && leftFootGround == null)
            collider = rightFootGround;
        else if (rightFootGround != null && leftFootGround != null)
        {
            if ((movementDirX >= 0 && player.localEulerAngles.y == 0) || (movementDirX == -1 && player.localEulerAngles.y == 180))
                collider = rightFootGround;
            else
                collider = leftFootGround;
        }

        if (collider)
        {
            groundAngle = collider.transform.eulerAngles.z;
            float tangent = Mathf.Tan(groundAngle * Mathf.PI / 180);
            Vector2 dir = new Vector2(1, tangent).normalized;

            groundDir = dir;
        }
        else
        {
            groundAngle = 0;
            groundDir = new Vector2(1, 0);
        }

        return (rightFootGround || leftFootGround) ? true : false;
    }

    //foot collider becomes smaller when jumping
    private IEnumerator handleColliders()
    {
        yield return new WaitForSeconds(0.03f);

        //disable main foot's collider when not walking
        footCollider.gameObject.SetActive(animator.GetInteger("Phase") == 1);

        //tuck the feet ground raycasters in when jumping
        rightFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.99f, rightFoot.transform.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.transform.localPosition.y, 0);

        leftFoot.transform.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.357f, leftFoot.transform.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.transform.localPosition.y, 0);

        //thin collider when jumping
        mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.68f : 1f, mainCollider.size.y);

        //shorten collider when double jumping
        mainCollider.size = new Vector2(mainCollider.size.x, animator.GetBool("double jump") && !stopSpinning ? 2f : 3.14f);
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

        //get animation progress (as a positive number btwn 0-1 regardless of whether the animation is played forwards or backwards)
        float t = ((animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) + 1) % 1;

        //if walking
        if (animator.GetInteger("Phase") == 1)
        {
            //go idle 
            if (newMode == 0 && ((t >= 0.31f && t <= 0.41f) || (t >= 0.8633f && t <= 0.975f)))
                StartCoroutine(walkingToIdle());

            //go jump
            if (newMode == 2)
                animator.SetInteger("Phase", 2);
        }

        else
            animator.SetInteger("Phase", newMode);
    }

    //Submethod that adds tiny delay before switching from walking to idle animation (cleaner transition from walking to other non-idle animations)
    private IEnumerator walkingToIdle()
    {
        yield return new WaitForSeconds(0.045f);

        if (movementDirX != 0)
            yield return null;
        else
            animator.SetInteger("Phase", 0);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            touchingMap = false;
    }

    private void FixedUpdate()
    {
        if (animator.GetBool("double jump"))
        {
            if (!stopSpinning || (transform.eulerAngles.z > 3 && transform.eulerAngles.z < 357))
                transform.eulerAngles = new Vector3(0, 0, (transform.eulerAngles.z + Time.deltaTime * spinRate * spinDirection));

        }
    }

    private IEnumerator timeDoubleSpin()
    {
        leftFoot.gameObject.SetActive(false);
        rightFoot.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.6f);
        stopSpinning = true;
        spinRate = 200;

        yield return new WaitForSeconds(0.1f);
        disableLimbs = false;
        leftFoot.gameObject.SetActive(true);
        rightFoot.gameObject.SetActive(true);
    }

    private void debugFrameRate()
    {
        frames++;
        time += Time.deltaTime;

        if (time >= 1.4f)
        {
            Debug.Log(frames / time);
            time = 0;
            frames = 0;
        }
    }
}
