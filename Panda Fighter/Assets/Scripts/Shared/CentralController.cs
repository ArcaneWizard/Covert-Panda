using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralController : MonoBehaviour
{
    protected Rigidbody2D rig;
    protected Transform body;
    protected Animator animator;

    [Header("Limbs and colliders")]
    public Transform shootingArm;
    public Transform head;
    public BoxCollider2D mainCollider;
    public BoxCollider2D footCollider;

    [Header("Camera stuff")]
    public Camera camera;

    [Header("Ground detection")]
    public Transform leftFoot;
    public Transform rightFoot;
    public Transform groundColliders;

    protected float maxSpeed = 13.0f;
    protected float speed;
    protected float jumpForce = 1130f;
    protected float doublejumpForce = 1243f;

    protected RaycastHit2D leftGroundHit, rightGroundHit;
    protected GameObject leftFootGround, rightFootGround;
    protected float lastGroundAngle;

    protected Vector2 obstacleToTheLeft, obstacleToTheRight;
    protected bool wallToTheLeft, wallToTheRight;

    public bool isGrounded { get; private set; }
    public bool isTouchingMap { get; private set; }
    protected float groundAngle;
    protected Vector2 groundDir;
    protected bool checkForAngle;

    public int dirX;
    protected float zAngle;

    //ideal aim coordinates when looking to the side, up or down 
    [HideInInspector]
    public Vector2 pointingRight, pointingUp, pointingDown, shoulderPos;
    protected float upVector, downVector, rightVector;
    protected float up, right, down;

    [HideInInspector]
    public Transform aimTarget;

    protected CentralWeaponSystem weaponSystem;
    protected CentralWeaponAttacks weaponAttacks;
    protected CentralAnimationController controller;

    public void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        body = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        weaponAttacks = transform.GetComponent<CentralWeaponAttacks>();
        controller = transform.GetComponent<CentralAnimationController>();

        speed = maxSpeed;
    }

    public virtual void Start()
    {
        StartCoroutine(findWalls());
        StartCoroutine(determineIfGrounded(controller.disableLimbsDuringDoubleJump));
    }

    //player or alien's body should tilt slightly on the slanted platform
    protected void tilt()
    {
        zAngle = transform.eulerAngles.z;

        if (zAngle > 180)
            zAngle = zAngle - 360;

        if (isGrounded && (dirX != 0 || (dirX == 0 && groundAngle == lastGroundAngle)))
        {
            float newGroundAngle = ((groundAngle - 360) / 1.4f);

            if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f && groundAngle <= 180)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (groundAngle / 1.4f - zAngle) * 20 * Time.deltaTime);
            else if (Mathf.Abs(groundAngle - transform.eulerAngles.z) > 0.5f)
                transform.eulerAngles = new Vector3(0, 0, zAngle + (newGroundAngle - zAngle) * 20f * Time.deltaTime);
        }
        else if (!isGrounded && Mathf.Abs(transform.eulerAngles.z) > 0.5f && !animator.GetBool("double jump"))
            transform.eulerAngles = new Vector3(0, 0, zAngle - zAngle * 10 * Time.deltaTime);
    }

    protected void rotateHeadAndWeapon(Vector2 shootDirection, float shootAngle, bool disableAiming)
    {
        if (shootDirection.y >= 0)
        {
            float slope = (up - right) / 90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (upVector - rightVector) / 90f;
            float weaponDirMagnitude = shootAngle * dirSlope + rightVector;

            Vector2 targetLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            if (aimTarget && !disableAiming)
                aimTarget.transform.localPosition = targetLocation;

            float headSlope = (122f - 92.4f) / 90f;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
        }

        if (shootDirection.y < 0)
        {
            float slope = (down - right) / -90f;
            float weaponRotation = shootAngle * slope + right;

            float dirSlope = (downVector - rightVector) / -90f;
            float weaponDirMagnitude = shootAngle * dirSlope + rightVector;

            Vector2 targetLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
            if (aimTarget && !disableAiming)
                aimTarget.transform.localPosition = targetLocation;

            float headSlope = (67f - 92.4f) / -90f;
            head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
        }
    }


    //check if the creature is on the ground + update the groundAngle
    public IEnumerator determineIfGrounded(bool disableLimbsDuringDoubleJump)
    {
        //use raycasts to check for ground below the left foot and right foot (+ draw raycasts for debugging)
        leftGroundHit = Physics2D.Raycast(leftFoot.position, Vector2.down, 2f, Constants.map);
        leftFootGround = (leftGroundHit.collider != null && leftGroundHit.normal.y >= 0.3f) ? leftGroundHit.collider.gameObject : null;
        //Vector2 leftGround = (leftGroundHit.collider != null) ? leftGroundHit.point : new Vector2(leftFoot.position.x, leftFoot.position.y) + Vector2.down * 2;
        //Debug.DrawLine(new Vector2(leftFoot.position.x, leftFoot.position.y), leftGround, Color.green, 2f);

        rightGroundHit = Physics2D.Raycast(rightFoot.position, Vector2.down, 2f, Constants.map);
        rightFootGround = (rightGroundHit.collider != null && rightGroundHit.normal.y >= 0.3f) ? rightGroundHit.collider.gameObject : null;
        //Vector2 rightGround = (rightGroundHit.collider != null) ? rightGroundHit.point : new Vector2(rightFoot.position.x, rightFoot.position.y) + Vector2.down * 2;
        //Debug.DrawLine(new Vector2(rightFoot.position.x, rightFoot.position.y), rightGround, Color.cyan, 2f);

        //if the creature was just grounded after falling OR the player been moving on the ground, enable this bool (used later to prevent bug)
        if ((!isGrounded || dirX != 0) && (leftFootGround || rightFootGround))
            checkForAngle = true;

        //determine if creature is grounded if either foot raycast hit the ground
        if (!disableLimbsDuringDoubleJump)
            isGrounded = (leftFootGround || rightFootGround) ? true : false;

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

        //reupdate the ground angle after 0.14 seconds
        yield return new WaitForSeconds(0.14f);
        StartCoroutine(determineIfGrounded(disableLimbsDuringDoubleJump));
    }

    protected IEnumerator findWalls()
    {
        if (body.localEulerAngles.y == 0)
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(leftFoot.position, -groundDir, 2f, Constants.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //obstacleToTheLeft = (leftWallHit.collider != null) ? leftWallHit.point : new Vector2(leftFoot.position.x, leftFoot.position.y) - groundDir * 2;
            //Debug.DrawLine(new Vector2(leftFoot.position.x, leftFoot.position.y), obstacleToTheLeft, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(rightFoot.position, groundDir, 2f, Constants.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //obstacleToTheRight = (rightWallHit.collider != null) ? rightWallHit.point : new Vector2(rightFoot.position.x, rightFoot.position.y) + groundDir * 2;
            //Debug.DrawLine(new Vector2(rightFoot.position.x, rightFoot.position.y), obstacleToTheRight, Color.red, 2f);
        }
        else
        {
            RaycastHit2D leftWallHit = Physics2D.Raycast(rightFoot.position, -groundDir, 2f, Constants.map);
            wallToTheLeft = (leftWallHit.collider != null && leftWallHit.normal.y < 0.3f) ? true : false;
            //obstacleToTheLeft = (leftWallHit.collider != null) ? leftWallHit.point : new Vector2(rightFoot.position.x, rightFoot.position.y) - groundDir * 2;
            //Debug.DrawLine(new Vector2(rightFoot.position.x, rightFoot.position.y), obstacleToTheLeft, Color.blue, 2f);

            RaycastHit2D rightWallHit = Physics2D.Raycast(leftFoot.position, groundDir, 2f, Constants.map);
            wallToTheRight = (rightWallHit.collider != null && rightWallHit.normal.y < 0.3f) ? true : false;
            //obstacleToTheRight = (rightWallHit.collider != null) ? rightWallHit.point : new Vector2(leftFoot.position.x, leftFoot.position.y) + groundDir * 2;
            //Debug.DrawLine(new Vector2(leftFoot.position.x, leftFoot.position.y), obstacleToTheRight, Color.red, 2f);
        }

        yield return new WaitForSeconds(0.33f);
        StartCoroutine(findWalls());
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

    public void calculateShoulderAngles(List<Vector2> aiming)
    {
        //get specific weapon aim coordinates
        pointingRight = aiming[0];
        pointingUp = aiming[1];
        pointingDown = aiming[2];
        shoulderPos = aiming[3];

        //ideal angle from shoulder to specific gun coordinates
        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //ideal vector magnitudes from shoulder to specific gun coordinates
        upVector = (pointingUp - shoulderPos).magnitude;
        rightVector = (pointingRight - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
    }

}
