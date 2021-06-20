
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
    public Transform legs;
    public BoxCollider2D mainCollider;

    public Transform gun;
    public BoxCollider2D footCollider;

    public Camera camera;
    private Vector3 cameraOffset;

    private float speed = 8.0f;
    private float jumpForce = 600;

    public Transform leftFoot;
    public Transform rightFoot;

    [SerializeField]
    private bool grounded;
    [SerializeField]
    private bool touchingMap;
    [SerializeField]
    private float groundAngle;
    private Vector2 groundDir;

    private int movementDirX;

    private float timer;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        player = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();

        cameraOffset = camera.transform.position - transform.position;
    }

    void Update()
    {
        camera.transform.position = transform.position + cameraOffset;
        grounded = isGrounded();

        playerLimbsOrientation();
        playerAnimationController();
        handlefootCollider();

        //use A and D keys for left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        //use W and S keys for jumping up or thrusting downwards
        if (Input.GetKeyDown(KeyCode.W) && grounded)
        {
            rig.AddForce(new Vector2(0, jumpForce));
            animator.SetBool("jumped", true);
        }
        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        //player velocity is aligned parallel to the slanted ground whenever the player is on a surface
        if (touchingMap && !animator.GetBool("jumped") && grounded)
            rig.velocity = groundDir * speed * movementDirX;
        else
            rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);

        //player's body tilts slightly to align with the slanted platform
        float zAngle = transform.eulerAngles.z;
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
        else if (!grounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f)
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void playerLimbsOrientation()
    {
        //player faces left or right depending on mouse cursor
        if (Input.mousePosition.x >= camera.WorldToScreenPoint(shootingArm.parent.position).x)
            player.localRotation = Quaternion.Euler(0, 0, 0);
        else
            player.localRotation = Quaternion.Euler(0, 180, 0);

        //calculate the angle btwn mouse cursor and player's shooting arm
        Vector2 shootDirection = (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
        float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

        //ideal local gun coordinates when looking to the side, up or down 
        Vector2 pointingRight = new Vector2(0.817f, 2.077f);
        Vector2 pointingUp = new Vector2(-0.276f, 3.389f);
        Vector2 pointingDown = new Vector2(-0.548f, 0.964f);
        Vector2 shoulderPos = new Vector2(-0.434f, 2.128f);

        //ideal angle from shoulder to the above gun coordinate
        float up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        float right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        float down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        if (shootDirection.y >= 0)
        {
            float slope = (up - right) / 90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (1.252f - 1.271f) / 90f;
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

            float dirSlope = (1.17f - 1.271f) / -90f;
            float weaponDirMagnitude = shootAngle * dirSlope + 1.271f;

            Vector2 gunLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            gun.transform.localPosition = gunLocation;

            float headSlope = (67f - 92.4f) / -90f;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
        }
    }

    //states when to transition btwn diff player animation states 
    private void playerAnimationController()
    {
        //if the player isn't currently midair (ie. isn't in the jump animation state) 
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
            setAnimation("idle");
        }
    }

    //check if the player is on the ground + update the groundAngle
    public bool isGrounded()
    {
        RaycastHit2D leftFootGrounded = Physics2D.Raycast(leftFoot.position, Vector2.down, 0.3f, Constants.map);
        RaycastHit2D rightFootGrounded = Physics2D.Raycast(rightFoot.position, Vector2.down, 0.3f, Constants.map);

        GameObject collider = null;

        if (leftFootGrounded.collider != null && rightFootGrounded.collider == null)
            collider = leftFootGrounded.collider.gameObject;
        else if (rightFootGrounded.collider != null && leftFootGrounded.collider == null)
            collider = rightFootGrounded.collider.gameObject;
        else if (rightFootGrounded.collider != null && leftFootGrounded.collider != null)
        {
            RaycastHit2D centerOfMassGrounded = Physics2D.Raycast(transform.position, Vector2.down, 3.22f, Constants.map);

            if (centerOfMassGrounded.collider != null)
            {
                Debug.Log("center of mass taken");
                collider = centerOfMassGrounded.collider.gameObject;
            }
        }

        /*if (leftFootGrounded.collider != null)
        {
            collider = leftFootGrounded.collider.gameObject;

            //if the ground below the right foot is steeper than the ground below the left foot
            if (rightFootGrounded.collider != null && Mathf.Abs(180 - rightFootGrounded.collider.transform.eulerAngles.z) >= Mathf.Abs(180 - leftFootGrounded.collider.transform.eulerAngles.z))
                collider = rightFootGrounded.collider.gameObject;
        }
        else if (rightFootGrounded.collider != null)
            collider = rightFootGrounded.collider.gameObject;*/

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

        return (rightFootGrounded || leftFootGrounded) ? true : false;
    }

    //foot collider becomes smaller when jumping
    private void handlefootCollider()
    {
        //disable main foot's collider when not walking
        footCollider.enabled = animator.GetInteger("Phase") == 1;

        //tuck the feet ground raycasters in when jumping
        rightFoot.transform.localPosition = footCollider.enabled
        ? new Vector3(0.99f, rightFoot.transform.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.transform.localPosition.y, 0);

        leftFoot.transform.localPosition = footCollider.enabled
        ? new Vector3(-0.357f, leftFoot.transform.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.transform.localPosition.y, 0);

        //thin collider when jumping
        mainCollider.size = new Vector2(animator.GetInteger("Phase") == 2 ? 0.68f : 1f, mainCollider.size.y);
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
}
