using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralController : MonoBehaviour
{
    protected Rigidbody2D rig;
    protected Transform body;
    protected Animator animator;
    protected CentralAnimationController animController;

    [Header("Limbs & Colliders")]
    public Transform shootingArm;
    public BoxCollider2D mainCollider;
    public BoxCollider2D footCollider;

    [Header("Camera stuff")]
    public Camera camera;

    [Header("Ground detection")]
    public Transform leftFoot;
    public Transform rightFoot;

    public static float maxSpeed = 14.5f;
    public static float jumpForce = 1300f;
    public static float doubleJumpForce = 1200f;
    public static float launchBoostForce = 2500f;
    public static float maxGravity = 2.5f;

    public float speed { get; protected set; }
    public int dirX { get; protected set; }
    public bool isGrounded { get; private set; }
    public bool isTouchingMap { get; private set; }
    public bool wallToTheLeft { get; private set; }
    public bool wallToTheRight { get; private set; }

    // variables utilized in inherited classes
    protected GameObject leftGround, rightGround, centerGround;
    protected Vector2 groundDir;

    // cached variables, used for repeated calculations 
    private float groundAngle, zAngle, lastGroundAngle;
    private RaycastHit2D leftGroundHit, rightGroundHit, centerGroundHit;
    private Vector2 closerLeftWallFoot, closerRightWallFoot;
    private IdleTilt idleTilt;
    private float idleGroundAngle;

    public void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        animController = transform.GetComponent<CentralAnimationController>();

        speed = maxSpeed;
    }

    public virtual void Start()
    {
        StartCoroutine(updateWallChecks(1.2f));
    }

    public virtual void Update()
    {
        updateIfGrounded(animController.disableLimbsDuringDoubleJump);
        // tilt();
    }

    public void setSpeed(float speed) => this.speed = speed;
    public void setDirection(int dir) => this.dirX = dir;

    // perform launch boost when jumping off a launch pad 
    public void launchBoost()
    {
        rig.velocity = new Vector2(rig.velocity.x, 0);
        rig.gravityScale = maxGravity;
        rig.AddForce(new Vector2(0, launchBoostForce));
        animator.SetBool("jumped", true);
    }

    // player or alien's body should tilt slightly on the slanted platform. Called every frame
    private void tilt()
    {
        zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle = zAngle - 360;

        // While moving and grounded, the player should rotate to the correct tilt based off the ground's angle 
        if (isGrounded && dirX != 0)
        {
            float newGroundAngle = ((groundAngle - 360) / 1.4f);

            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f && groundAngle <= 180)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (groundAngle / 1.4f - zAngle) * 20 * Time.deltaTime);


            else if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20f * Time.deltaTime);
        }

        // While still and grounded, creature should rotate to the correct tilt based off the ground's angle. Uses 
        // the float idleGroundAngle, whose value is updated just once, to prevent a bug (spasming on uneven ground while still)
        else if (isGrounded && dirX == 0 && idleTilt == IdleTilt.set)
        {
            float newIdleGroundAngle = ((idleGroundAngle - 360) / 1.4f);

            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f && idleGroundAngle <= 180)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (idleGroundAngle / 1.4f - zAngle) * 20 * Time.deltaTime);

            else if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newIdleGroundAngle - zAngle) * 20f * Time.deltaTime);
        }

        // when the player is not grounded or not touching any platform, it should turn back upright really quickly 
        else if ((!isGrounded || !isTouchingMap) && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !animator.GetBool("double jump"))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 22 * Time.deltaTime);

        // reset the idle tilt when the player moves, so that a new idle ground angle will be updated in the future
        if ((dirX != 0) && idleTilt == IdleTilt.set)
            idleTilt = IdleTilt.reset;
    }

    // update whether or not the creature is on the ground (bool) and the ground angle
    private void updateIfGrounded(bool disableLimbsDuringDoubleJump)
    {
        // use raycasts to check for ground below the left foot and right foot (+ draw raycasts for debugging)
        leftGroundHit = Physics2D.Raycast(leftFoot.position, Vector2.down, 2f, Constants.map);
        leftGround = leftGroundHit.collider != null ? leftGroundHit.collider.gameObject : null;
        //Vector2 leftGround = (leftGroundHit.collider != null) ? leftGroundHit.point : new Vector2(leftFoot.position.x, leftFoot.position.y) + Vector2.down * 2;
        //Debug.DrawLine(new Vector2(leftFoot.position.x, leftFoot.position.y), leftGround, Color.green, 2f);

        rightGroundHit = Physics2D.Raycast(rightFoot.position, Vector2.down, 2f, Constants.map);
        rightGround = rightGroundHit.collider != null ? rightGroundHit.collider.gameObject : null;
        //Vector2 rightGround = (rightGroundHit.collider != null) ? rightGroundHit.point : new Vector2(rightFoot.position.x, rightFoot.position.y) + Vector2.down * 2;
        //Debug.DrawLine(new Vector2(rightFoot.position.x, rightFoot.position.y), rightGround, Color.cyan, 2f);

        // update whether or not the creature is grounded. It's grounded when there is ground below  
        // one foot and the ground's angle is inclined less than 70 degrees. Now if there's ground 
        // below both feet, then it's definitely not falling into a wall, so it's grounded as long as
        // the ground is inclined less than 84 degrees 
        if (!disableLimbsDuringDoubleJump)
        {
            isGrounded =
                (leftGround && leftGroundHit.normal.y > 0.34f) ||
                (rightGround && rightGroundHit.normal.y > 0.34f) ||
                (leftGround && leftGroundHit.normal.y > 0.1f && rightGround && rightGroundHit.normal.y > 0.1f);
        }


        DebugGUI.debugText9 = $"{leftGroundHit.normal.y}, {rightGroundHit.normal.y}, {isGrounded}  ";
        // register the angle of the ground
        if (isGrounded)
        {
            // moving right while facing right or moving left while facing left -> use "right foot" gameobject
            if (((dirX == 1 && body.localEulerAngles.y == 0) || (dirX == -1 && body.localEulerAngles.y == 180)) && rightGround
                && rightGroundHit.normal.y > 0.2f)
            {
                groundDir = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            // moving left while facing right or moving right facing left -> use "left foot" gameobject
            else if (((dirX == -1 && body.localEulerAngles.y == 0) || (dirX == 1 && body.localEulerAngles.y == 180)) && leftGround
                && leftGroundHit.normal.y > 0.2f)
            {
                groundDir = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            else if (dirX != 0 && leftGroundHit.normal.y > rightGroundHit.normal.y)
            {
                groundDir = new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            else if (dirX != 0)
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

        // if the creature just went idle while grounded, it's ground angle will only be set once
        // fixes bug: player sometimes spasm when standing still on uneven ground 
        if (dirX == 0 && isGrounded && idleTilt == IdleTilt.reset)
        {
            idleTilt = IdleTilt.set;
            idleGroundAngle = groundAngle;
        }
    }

    // update whether there's a wall to entity's left and right within the provided distance.
    // bools updated 3 times a second
    private IEnumerator updateWallChecks(float wallDistance)
    {
        // update position of the foot closest to left wall + position of the foot closest to the right wall 
        closerLeftWallFoot = (body.localEulerAngles.y == 0) ? leftFoot.position : rightFoot.position;
        closerRightWallFoot = (body.localEulerAngles.y == 0) ? rightFoot.position : leftFoot.position;

        RaycastHit2D leftWallHit = Physics2D.Raycast(closerLeftWallFoot, -groundDir, wallDistance, Constants.map);
        wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
        //obstacleToTheLeft = (leftWallHit.collider != null) ? leftWallHit.point : new Vector2(leftFoot.position.x, leftFoot.position.y) - groundDir * 2;
        //Debug.DrawLine(new Vector2(leftFoot.position.x, leftFoot.position.y), obstacleToTheLeft, Color.blue, 2f);

        RaycastHit2D rightWallHit = Physics2D.Raycast(closerRightWallFoot, groundDir, wallDistance, Constants.map);
        wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
        //obstacleToTheRight = (rightWallHit.collider != null) ? rightWallHit.point : new Vector2(rightFoot.position.x, rightFoot.position.y) + groundDir * 2;
        //Debug.DrawLine(new Vector2(rightFoot.position.x, rightFoot.position.y), obstacleToTheRight, Color.red, 2f);x

        yield return new WaitForSeconds(0.33f);
        StartCoroutine(updateWallChecks(wallDistance));
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            isTouchingMap = true;
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            isTouchingMap = true;
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == 11)
            isTouchingMap = false;
    }

    //when the player is idle, their tilt is only set once to prevent them from spasming on uneven ground while still
    private enum IdleTilt
    {
        set, reset
    }
}
