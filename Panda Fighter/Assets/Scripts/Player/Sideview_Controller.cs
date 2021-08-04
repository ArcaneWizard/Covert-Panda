
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
    private WeaponSystem weaponSystem;
    private WeaponAttacks weaponAttacks;
    private AnimationController controller;

    [Header("Limbs and colliders")]
    public Transform shootingArm;
    public Transform head;
    public BoxCollider2D mainCollider;
    public BoxCollider2D footCollider;
    public Transform groundColliders;
    [HideInInspector]
    public string nextToWall;

    [Header("Camera stuff")]
    public Camera camera;
    public Transform alienToFollow;

    private float speed = 10.0f;
    private float jumpForce = 1130f;

    [Header("Ground detection")]
    public Transform leftFoot;
    public Transform rightFoot;
    private RaycastHit2D leftGroundHit, rightGroundHit;
    private GameObject leftFootGround, rightFootGround;
    private float lastGroundAngle;

    private Vector2 obstacleToTheLeft, obstacleToTheRight;
    private bool wallToTheLeft, wallToTheRight;

    public bool grounded;
    private bool touchingMap;
    private float groundAngle;
    private Vector2 groundDir;
    private bool checkForAngle;

    [HideInInspector]
    public int movementDirX;
    private float zAngle;

    private float time = 0;
    private float frames = 0;

    //ideal aim coordinates when looking to the side, up or down 
    [HideInInspector]
    public Vector2 pointingRight, pointingUp, pointingDown, shoulderPos;
    private float upVector, downVector, rightVector;
    private float up, right, down;

    [HideInInspector]
    public Transform aimTarget;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        player = transform.GetChild(0).transform;
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        weaponSystem = transform.GetComponent<WeaponSystem>();
        weaponAttacks = transform.GetComponent<WeaponAttacks>();
        controller = transform.GetComponent<AnimationController>();

        StartCoroutine(findWalls());
        StartCoroutine(isGrounded());
    }

    void Update()
    {
        //use A and D keys for left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (Input.GetKeyDown(KeyCode.W) && animator.GetBool("jumped") && !animator.GetBool("double jump"))
        {
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, jumpForce * 1.1f));
            controller.startDoubleJumpAnimation();
        }

        if (Input.GetKeyDown(KeyCode.W) && grounded && !animator.GetBool("double jump"))
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
        lookAndAimInRightDirection();
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
            if (movementDirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, 0);

            //no x velocity when running into a wall to avoid bounce/fall glitch
            else if (movementDirX == -1 && wallToTheLeft)
                rig.velocity = new Vector2(0, 0);

            //player velocity is parallel to the slanted ground
            else
                rig.velocity = groundDir * speed * movementDirX;
        }

        //when player is not on the ground, player velocity is just left/right with gravity applied
        else
        {
            //no x velocity when running into a wall mid-air to avoid clipping glitch
            if (movementDirX == 1 && wallToTheRight)
                rig.velocity = new Vector2(0, rig.velocity.y);

            //no x velocity when running into a wall mid-air to avoid clipping glitch
            else if (movementDirX == -1 && wallToTheLeft)
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

        if (grounded && (movementDirX != 0 || (movementDirX == 0 && groundAngle == lastGroundAngle)))
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
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!controller.disableLimbsDuringDoubleJump)
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
                float weaponDirMagnitude = shootAngle * dirSlope + rightVector;

                Vector2 targetLocation = weaponDirMagnitude * new Vector2(Mathf.Cos(weaponRotation * Mathf.PI / 180f), Mathf.Sin(weaponRotation * Mathf.PI / 180f)) + shoulderPos;
                if (aimTarget && !weaponAttacks.disableAiming)
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
                if (aimTarget && !weaponAttacks.disableAiming)
                    aimTarget.transform.localPosition = targetLocation;

                float headSlope = (67f - 92.4f) / -90f;
                head.eulerAngles = new Vector3(head.eulerAngles.x, head.eulerAngles.y, headSlope * shootAngle + 92.4f);
            }
        }
    }

    //check if the player is on the ground + update the groundAngle
    public IEnumerator isGrounded()
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

        //if the player was just grounded after falling OR the player been moving on the ground, enable this bool (used later to prevent bug)
        if ((!grounded || movementDirX != 0) && (leftFootGround || rightFootGround))
            checkForAngle = true;

        //determine if player is grounded if either foot raycast hit the ground
        if (!controller.disableLimbsDuringDoubleJump)
            grounded = (leftFootGround || rightFootGround) ? true : false;

        //register the angle of the ground
        if (grounded)
        {
            //moving right while facing right or moving left while facing left -> use "right foot" gameobject
            if (((movementDirX == 1 && player.localEulerAngles.y == 0) || (movementDirX == -1 && player.localEulerAngles.y == 180)) && rightFootGround)
            {
                groundDir = new Vector2(rightGroundHit.normal.y, -rightGroundHit.normal.x);
                float f = groundDir.y / groundDir.x;
                groundAngle = Mathf.Atan(f) * 180f / Mathf.PI;
            }

            //moving left while facing right or moving right facing left -> use "left foot" gameobject
            else if (((movementDirX == -1 && player.localEulerAngles.y == 0) || (movementDirX == 1 && player.localEulerAngles.y == 180)) && leftFootGround)
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
        StartCoroutine(isGrounded());
    }

    private IEnumerator findWalls()
    {
        if (player.localEulerAngles.y == 0)
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

    public void calculateShoulderAngles()
    {
        //ideal angle from shoulder to specific gun coordinates
        up = Mathf.Atan2(pointingUp.y - shoulderPos.y, pointingUp.x - shoulderPos.x) * 180 / Mathf.PI;
        right = Mathf.Atan2(pointingRight.y - shoulderPos.y, pointingRight.x - shoulderPos.x) * 180 / Mathf.PI;
        down = Mathf.Atan2(pointingDown.y - shoulderPos.y, pointingDown.x - shoulderPos.x) * 180 / Mathf.PI;

        //ideal vector magnitudes from shoulder to specific gun coordinates
        upVector = (pointingUp - shoulderPos).magnitude;
        rightVector = (pointingRight - shoulderPos).magnitude;
        downVector = (pointingDown - shoulderPos).magnitude;
    }

    //Player is on a levitation boost platform and clicks W -> give them a jump boost 
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Levitation" && Input.GetKeyDown(KeyCode.W) && grounded)
            rig.AddForce(Constants.levitationBoost);
    }
}
