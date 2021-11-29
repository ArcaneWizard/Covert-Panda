using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralController : MonoBehaviour
{
    protected Rigidbody2D rig;
    protected Transform body;
    protected Animator animator;
    protected CentralAnimationController animController;
    protected IK_Foot iK_Foot;

    [Header("Limbs & Colliders")]
    public Transform shootingArm;
    public Detector touchingMap;
    public Detector leftWall;
    public Detector rightWall;

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

    public static int maxSlopeAngle = 64; //max slope angle you can climb b4 slipping down
    private float maxSlope_Y;

    public float speed { get; protected set; }
    public int dirX { get; protected set; }
    public bool isGrounded { get; private set; }
    public bool isTouchingMap { get; private set; }
    public bool wallToTheLeft { get; private set; }
    public bool wallToTheRight { get; private set; }

    // variables utilized in inherited classes
    public GameObject leftGround { get; protected set; }
    public GameObject rightGround { get; protected set; }
    public GameObject centerGround { get; protected set; }
    public Vector2 groundDir { get; protected set; }
    public float groundAngle { get; protected set; }

    //colliders
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D capsuleCollider;

    // cached variables, used for repeated calculations 
    private float zAngle;
    private RaycastHit2D leftGroundHit, rightGroundHit, centerGroundHit;
    private Vector2 closerLeftWallFoot, closerRightWallFoot;

    public void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        iK_Foot = transform.GetChild(0).transform.GetComponent<IK_Foot>();
        animController = transform.GetComponent<CentralAnimationController>();
        boxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        capsuleCollider = transform.GetChild(0).GetComponent<CapsuleCollider2D>();

        speed = maxSpeed;
        maxSlope_Y = Mathf.Cos(maxSlopeAngle);
    }

    public virtual void Update()
    {
        updateIfGrounded(animController.disableLimbsDuringDoubleJump);
        isTouchingMap = touchingMap.detected;
        wallToTheLeft = facingRight ? leftWall.detected : rightWall.detected;
        wallToTheRight = facingRight ? rightWall.detected : leftWall.detected;

        StartCoroutine(adjustCollidersBasedOnState());
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

    // update whether or not the creature is on the ground (bool) and the ground angle
    private void updateIfGrounded(bool disableLimbsDuringDoubleJump)
    {
        leftGroundHit = Physics2D.Raycast(leftFoot.position, Vector2.down, 2.7f, Constants.map);
        leftGround = leftGroundHit.collider != null ? leftGroundHit.collider.gameObject : null;

        rightGroundHit = Physics2D.Raycast(rightFoot.position, Vector2.down, 2.7f, Constants.map);
        rightGround = rightGroundHit.collider != null ? rightGroundHit.collider.gameObject : null;

        // update whether or not the creature is grounded. 
        if (!disableLimbsDuringDoubleJump)
        {
            isGrounded =
                (leftGround && leftGroundHit.normal.y > maxSlope_Y) ||
                (rightGround && rightGroundHit.normal.y > maxSlope_Y);
        }

        DebugGUI.debugText1 = $"Grounded: {isGrounded}, touchingMap: {isTouchingMap}";

        // register the angle of the ground
        if (isGrounded)
        {
            bool movingRight = (dirX == 1 && facingRight) || (dirX == -1 && !facingRight);
            bool movingLeft = (dirX == -1 && facingRight) || (dirX == 1 && !facingRight);

            if (movingRight && rightGround && rightGroundHit.normal.y > maxSlope_Y)
                setGroundDirAndAngle(new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x));

            else if (movingLeft && leftGround && leftGroundHit.normal.y > maxSlope_Y)
                setGroundDirAndAngle(new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x));

            else if (leftGroundHit.normal.y > rightGroundHit.normal.y)
                setGroundDirAndAngle(new Vector2(leftGroundHit.normal.y, -leftGroundHit.normal.x));

            else
                setGroundDirAndAngle(new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x));
        }

        else
        {
            groundDir = new Vector2(1, 0);
            groundAngle = 90f;
        }

        DebugGUI.debugText2 = $"leftGroundNormal: {leftGroundHit.normal.y}, rightGroundNormal: {rightGroundHit.normal.y}";
        DebugGUI.debugText3 = $"groundDir: {groundDir}, groundAngle: {groundAngle}";
    }

    // Helper method ot set ground direction and angle
    private void setGroundDirAndAngle(Vector2 dir)
    {
        groundDir = dir;
        groundAngle = Mathf.Atan(groundDir.y / groundDir.x) * 180f / Mathf.PI;
    }

    private bool facingRight => body.localEulerAngles.y == 0;

    //foot collider becomes smaller when jumping
    protected IEnumerator adjustCollidersBasedOnState()
    {
        yield return new WaitForSeconds(0.03f);
        /*//tuck the feet ground raycasters in when jumping
        rightFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(0.99f, rightFoot.localPosition.y, 0)
        : new Vector3(0.332f, rightFoot.localPosition.y, 0);

        leftFoot.localPosition = animator.GetInteger("Phase") != 2
        ? new Vector3(-0.357f, leftFoot.localPosition.y, 0)
        : new Vector3(-0.157f, leftFoot.localPosition.y, 0);*/

        //thin collider when jumping
        bool jumping = animController.AnimatorHandler.IsPlaying(Animation.jumping);
        boxCollider.size = new Vector2(jumping ? 0.6f : 0.68f, boxCollider.size.y);
        capsuleCollider.size = new Vector2(jumping ? 0.6f : 0.7f, capsuleCollider.size.y);

        //shorten collider when double jumping
        boxCollider.size = new Vector2(boxCollider.size.x, animator.GetBool("double jump") ? 2f : 2.55f);
        capsuleCollider.size = new Vector2(capsuleCollider.size.x, animator.GetBool("double jump") ? 0.1f : 1.46f);
    }
}
