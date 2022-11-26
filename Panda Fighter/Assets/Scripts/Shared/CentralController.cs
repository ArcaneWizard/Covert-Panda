using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the creature's movement, including related tasks like ground raycasting,
// wall detection, and jumping.

public class CentralController : MonoBehaviour
{
    protected Rigidbody2D rig;
    protected Transform body;
    protected CentralAnimationController controller;
    protected Health health;
    public Animator animator { get; private set; }

    [Header("Limbs and colliders")]
    public Transform shootingArm;
    public BoxCollider2D mainCollider;
    public BoxCollider2D oneWayCollider;

    [Header("Ground detection")]
    public Transform leftGroundChecker;
    public Transform rightGroundChecker;
    public Transform physicalLeftFoot;
    public Transform physicalRightFoot;

    // Useful movement information
    public bool isGrounded { get; protected set; }
    public bool isTouchingMap { get; protected set; }
    public float maxSpeed { get; private set; }
    protected float speed; // current speed of creature
    public int dirX { get; protected set; } // current movement dir of creature

    // Important movement constants:
    public static float jumpForce = 1750f; 
    public static float doubleJumpForce = 1850f; 
    public static float jumpPadForce = 3400; 
    public static float maxGravity = 5f;

    // Info about the ground or walls detected:
    protected RaycastHit2D leftGroundHit, rightGroundHit, centerGroundHit;
    protected GameObject leftFootGround, rightFootGround, centerGround;
    protected bool wallToTheLeft, wallToTheRight;

    // Info about the ground angle/current tilt of the creature on a platform
    protected float lastGroundAngle;
    public bool forceUpdateTilt;
    protected float groundAngle;
    protected Vector2 groundDir;
    protected bool checkForAngle;

    protected float zAngle;

    public void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        controller = transform.GetComponent<CentralAnimationController>();
        health = transform.GetComponent<Health>();

        Side side = transform.parent.GetComponent<Role>().side;
        mainCollider.gameObject.layer = (side == Side.Friendly) ? Layers.Friend : Layers.Enemy;
        mainCollider.offset = new Vector2(0, 1.45f);

        maxSpeed = 22f;
        speed = maxSpeed;
    }

    public virtual void Start()
    {
        StartCoroutine(findWalls());
        StartCoroutine(determineIfGrounded(controller.IsExecutingDoubleJump));
    }

    public virtual void Update()
    {
        if (speed < 0)
            Debug.LogError("speed should never be negative. Set dirX to negative instead");
        tilt();
    }

    public void setDirection(int dir) => this.dirX = dir;

    //player or alien's body should tilt slightly on the slanted platform
    protected void tilt()
    {
        //DebugGUI.debugTexts[7] = Bullet.raycastsUsed.ToString();
        zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle = zAngle - 360;

        float newGroundAngle = groundAngle <= 180 ? groundAngle / 2.2f : ((groundAngle - 360) / 2.2f);

        if (isGrounded && (dirX != 0 || (dirX == 0 && groundAngle == lastGroundAngle)))
        {
            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20 * Time.deltaTime);
        }

        else if (!isGrounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !animator.GetBool("double jump"))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);

        if (forceUpdateTilt) 
        {
            transform.eulerAngles = new Vector3(0, 0, newGroundAngle);
            forceUpdateTilt = false;
        }

        float tempGroundAngle = (groundAngle <= 180f) ? groundAngle : groundAngle - 360;
        physicalLeftFoot.transform.localEulerAngles = new Vector3(0, 0, 90 + tempGroundAngle);
        physicalRightFoot.transform.localEulerAngles = new Vector3(0, 0, 90 + tempGroundAngle);
    }
    
    //check if the creature is on the ground + update the groundAngle
    public IEnumerator determineIfGrounded(bool disableLimbsDuringDoubleJump)
    {
        updateGroundAngle(disableLimbsDuringDoubleJump);
        yield return new WaitForSeconds(0.03f);
        StartCoroutine(determineIfGrounded(disableLimbsDuringDoubleJump));
    }

    public void updateGroundAngle(bool disableLimbsDuringDoubleJump) 
    {
        //use raycasts to check for ground below the left foot and right foot (+ draw raycasts for debugging)
        leftGroundHit = Physics2D.Raycast(leftGroundChecker.position, Vector2.down, 2f, LayerMasks.map);
        if (leftGroundHit.collider != null)
            leftGroundHit = Physics2D.Raycast(leftGroundChecker.position + 3 * Vector3.up, Vector2.down, 5f, LayerMasks.map);

        rightGroundHit = Physics2D.Raycast(rightGroundChecker.position, Vector2.down, 2f, LayerMasks.map);
        if (rightGroundHit.collider != null)
            rightGroundHit = Physics2D.Raycast(rightGroundChecker.position + 3 * Vector3.up, Vector2.down, 5f, LayerMasks.map);

        leftFootGround = (leftGroundHit.collider != null && leftGroundHit.normal.y >= 0.3f) ? leftGroundHit.collider.gameObject : null;
        rightFootGround = (rightGroundHit.collider != null && rightGroundHit.normal.y >= 0.3f) ? rightGroundHit.collider.gameObject : null;

        //if the creature was just grounded after falling OR the player been moving on the ground, enable this bool (used later to prevent bug)
        if ((!isGrounded || dirX != 0) && (leftFootGround || rightFootGround))
            checkForAngle = true;

        //determine if creature is grounded if either foot raycast hit the ground
        if (!disableLimbsDuringDoubleJump)
            isGrounded = leftFootGround || rightFootGround;

        //register the angle of the ground
        if (isGrounded)
        {
            //moving right while facing right or moving left while facing left -> use "right foot" gameobject
            if (((dirX == 1 && body.localEulerAngles.y == 0) || (dirX == -1 && body.localEulerAngles.y == 180)) && rightFootGround)
            {
                groundDir = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            //moving left while facing right or moving right facing left -> use "left foot" gameobject
            else if (((dirX == -1 && body.localEulerAngles.y == 0) || (dirX == 1 && body.localEulerAngles.y == 180)) && leftFootGround)
            {
                groundDir = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            //not moving 
            else if (rightFootGround && rightGroundHit.normal.y != 1)
            {
                groundDir = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            //not moving
            else if (leftFootGround)
            {
                groundDir = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            //not moving
            else
            {
                groundDir = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }
        }

        else
        {
            groundAngle = 0;
            groundDir = new Vector2(1, 0);
        }

        //lastGroundAngle is used elsewhere to prevent a bug: where the player sometimes spasms when standing still on uneven terrain
        if (checkForAngle)
        {
            lastGroundAngle = groundAngle;
            checkForAngle = false;
        }
    }

    protected IEnumerator findWalls()
    {
        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(leftGroundChecker.position, -groundDir, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(rightGroundChecker.position, groundDir, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(rightGroundChecker.position, -groundDir, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(leftGroundChecker.position, groundDir, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }

        yield return new WaitForSeconds(Time.deltaTime * 2);
        StartCoroutine(findWalls());
    }

    protected void normalJump() 
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;

        rig.AddForce(new Vector2(0, jumpForce));
        animator.SetBool("jumped", true);
    }

    protected void doubleJump() 
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;

        rig.AddForce(new Vector2(0, doubleJumpForce));
        StartCoroutine(controller.startDoubleJumpAnimation());
    }

    protected void jumpPadBoost()
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;
        rig.AddForce(new Vector2(0, jumpPadForce));
        animator.SetBool("jumped", true);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layers.DefaultGround || col.gameObject.layer == Layers.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layers.DefaultGround || col.gameObject.layer == Layers.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layers.DefaultGround || col.gameObject.layer == Layers.OneWayGround)
            isTouchingMap = false;
    }

    private void FixedUpdate() => oneWayCollider.enabled = rig.velocity.y < 0.1f;
}
