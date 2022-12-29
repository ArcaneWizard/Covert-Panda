using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implements the creature's movement (for running, jumping, double
// jumping, and getting a jump boost off jump pads). Stores useful
// movement info such as whether the creature is grounded or touching
// the map, and the direction it's moving

public abstract class CentralController : MonoBehaviour
{
    // Useful movement information:
    public bool isGrounded { get; protected set; }
    public bool isTouchingMap { get; protected set; }
    public bool recentlyJumpedOffGround {get; private set; }

    [Range(-25.0f, 25.0f)]
    public float offset;
    
    // Current direction of creature's movement (-1 = left, 0 = idle, 1 = right)
    public int dirX { get; protected set; } 

    // Important movement constants:
    public const float jumpForce = 1750f; 
    public const float doubleJumpForce = 1850f; 
    public const float jumpPadForce = 3400; 
    public const float maxGravity = 5f;
    
    protected Rigidbody2D rig;
    protected Transform body;
    protected CentralPhaseTracker phaseTracker;
    protected Health health;
    protected CentralLookAround lookAround;
    protected Animator animator;

    [Header("Limbs and colliders")]
    public Transform shootingArm;
    public BoxCollider2D mainCollider;
    public BoxCollider2D oneWayCollider;

    [Header("Ground detection")]
    public Transform leftGroundChecker;
    public Transform rightGroundChecker;
    public Transform physicalLeftFoot;
    public Transform physicalRightFoot;

    protected float maxSpeed;
    protected float speed; 

    // Info about the ground or walls detected:
    protected RaycastHit2D leftGroundHit, rightGroundHit, centerGroundHit;
    protected GameObject leftFootGround, rightFootGround, centerGround;
    protected bool wallToTheLeft, wallToTheRight;

    // Info about the slope of the ground 
    protected Vector2 groundSlope;
    protected float groundAngle;
    private float lastGroundAngle;

    private bool updateTiltInstantly;

    public void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        phaseTracker = transform.GetComponent<CentralPhaseTracker>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();

        Side side = transform.parent.GetComponent<Role>().side;
        mainCollider.gameObject.layer = (side == Side.Friendly) ? Layer.Friend : Layer.Enemy;
        mainCollider.offset = new Vector2(0, 1.45f);

        maxSpeed = 22f;
        speed = maxSpeed;
    }

    public virtual void Start()
    {
        StartCoroutine(performWallChecks());
        StartCoroutine(repeatedlyCheckIfGrounded());
    }

    public virtual void LateUpdate() 
    {
        if (health.isDead)
            return;

        updateTilt();
    }

    // Set the x direction of the creature's movement (1 = right, 0 = still, -1 = left)
    public void SetDirection(int dir) => this.dirX = dir;

    // Immediately update the creature's standing tilt for the current ground 
    public void UpdateTiltInstantly() => updateTiltInstantly = true;

    // Update the creature's standing tilt and feet rotation depending on the ground angle
    private void updateTilt()
    {
        if (updateTiltInstantly) {
            updateGroundAngle();
            updateTiltInstantly = false;
        }

        if (float.IsNaN(groundAngle)) 
            return;

        float zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle -= 360;

        float newGroundAngle = groundAngle <= 180 ? groundAngle / 1.9f : ((groundAngle - 360) / 1.9f);

        if (isGrounded && (dirX != 0 || (dirX == 0 && groundAngle == lastGroundAngle)))
        {
            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20 * Time.deltaTime);
        }

        else if (!isGrounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !phaseTracker.IsPhase(Phase.DoubleJumping))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);

        if (updateTiltInstantly) 
            transform.eulerAngles = new Vector3(0, 0, newGroundAngle);

        /*float tempGroundAngle = (groundAngle <= 180f) ? groundAngle : groundAngle - 360;
        
        if (phaseManager.IsIdle) 
        {
            float shoeAngle = tempGroundAngle / 1.9f - 9f;
            float xTheta = lookAround.facingRight() ? 0f : -180f;
            float zTheta = lookAround.facingRight() ? shoeAngle
                : 180 + shoeAngle -  2 * zAngle;

            physicalLeftFoot.transform.eulerAngles = new Vector3(xTheta, 0f, zTheta);
            physicalRightFoot.transform.eulerAngles = new Vector3(xTheta, 0f, zTheta);
        }*/
    }
    
    //check if the creature is on the ground + update the groundAngle
    private IEnumerator repeatedlyCheckIfGrounded()
    {
        updateGroundAngle();
        yield return new WaitForSeconds(0.06f);
        StartCoroutine(repeatedlyCheckIfGrounded());
    }

    // update the ground angle
    private void updateGroundAngle() 
    {
        // use raycasts to check for ground below the left foot and right foot (+ draw raycasts for debugging)
        leftGroundHit = Physics2D.Raycast(leftGroundChecker.position, Vector2.down, 2f, LayerMasks.map);
        if (leftGroundHit.collider != null)
            leftGroundHit = Physics2D.Raycast(leftGroundChecker.position + 3 * Vector3.up, Vector2.down, 5f, LayerMasks.map);

        rightGroundHit = Physics2D.Raycast(rightGroundChecker.position, Vector2.down, 2f, LayerMasks.map);
        if (rightGroundHit.collider != null)
            rightGroundHit = Physics2D.Raycast(rightGroundChecker.position + 3 * Vector3.up, Vector2.down, 5f, LayerMasks.map);

        leftFootGround = (leftGroundHit.collider != null && leftGroundHit.normal.y >= 0.3f) ? leftGroundHit.collider.gameObject : null;
        rightFootGround = (rightGroundHit.collider != null && rightGroundHit.normal.y >= 0.3f) ? rightGroundHit.collider.gameObject : null;

        // if the creature was just grounded after falling OR the player been moving on the ground, enable this bool (used later to prevent bug)
        bool checkForAngle = false;
        if ((!isGrounded || dirX != 0) && (leftFootGround || rightFootGround))
            checkForAngle = true;

        // determine if creature is grounded if either foot raycast hit the ground
        if (!phaseTracker.DisableLimbsDuringSomersault)
            isGrounded = leftFootGround || rightFootGround;

        // register the angle of the ground
        if (isGrounded)
        {
            // moving right while facing right or moving left while facing left -> use "right foot" gameobject
            if (((dirX == 1 && body.localEulerAngles.y == 0) || (dirX == -1 && body.localEulerAngles.y == 180)) && rightFootGround)
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // moving left while facing right or moving right facing left -> use "left foot" gameobject
            else if (((dirX == -1 && body.localEulerAngles.y == 0) || (dirX == 1 && body.localEulerAngles.y == 180)) && leftFootGround)
            {
                groundSlope = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving 
            else if (rightFootGround && rightGroundHit.normal.y != 1)
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving
            else if (leftFootGround)
            {
                groundSlope = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // not moving
            else
            {
                groundSlope = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundSlope.y / groundSlope.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }
        }

        else
        {
            groundAngle = 0;
            groundSlope = new Vector2(1, 0);
        }

        // lastGroundAngle will be used to prevent a bug where the player sometimes spasms when standing still on uneven terrain
        if (checkForAngle)
            lastGroundAngle = groundAngle;
    }

    private IEnumerator performWallChecks()
    {
        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(leftGroundChecker.position, -groundSlope, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(rightGroundChecker.position, groundSlope, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(rightGroundChecker.position, -groundSlope, 2f, LayerMasks.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * -groundDir, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(leftGroundChecker.position, groundSlope, 2f, LayerMasks.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //Debug.DrawRay(leftGroundChecker.position, 2 * groundDir, Color.red, 2f);
        }

        yield return new WaitForSeconds(Time.deltaTime * 2);
        StartCoroutine(performWallChecks());
    }

    protected void normalJump()
    {
        StartCoroutine(RecentlyJumpedOffGround());
        phaseTracker.EnterJumpPhase();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;
        rig.AddForce(new Vector2(0, jumpForce));
    }

    protected void doubleJump()
    {
        StartCoroutine(phaseTracker.EnterDoubleJumpPhase());

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;
        rig.AddForce(new Vector2(0, doubleJumpForce));
    }

    protected void jumpPadBoost()
    {
        StartCoroutine(RecentlyJumpedOffGround());
        phaseTracker.EnterJumpPhase();

        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;
        rig.AddForce(new Vector2(0, jumpPadForce));
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultGround || col.gameObject.layer == Layer.OneWayGround)
            isTouchingMap = false;
    }

    private void FixedUpdate() => oneWayCollider.enabled = rig.velocity.y < 0.1f;

    private IEnumerator RecentlyJumpedOffGround()
    {
        recentlyJumpedOffGround = true;
        yield return new WaitForSeconds(0.2f);
        recentlyJumpedOffGround = false;
    }

}
