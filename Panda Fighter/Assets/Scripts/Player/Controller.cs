
using System;
using System.Collections.Generic;

using UnityEngine;

public class Controller : CentralController
{
    private bool standingOnJumpPad;
    private bool isThrustingDownwards;
    private float fastestYVelocityRecorded;

    // whether player hit the input for a certain action on the current frame
    private bool jumpInput;
    private bool downThrustInput;

    public float MOVEMENT_ALTERATION_SPEED = 40f;
    public float SLIDE_DOWN_SPEED = 0.5f;

    [SerializeField] private CameraMovement2 cameraMovement;

    protected override void Start()
    {
        base.Start();
        isThrustingDownwards = true;
    }

    protected override void Update()
    {
        base.Update();

        if (health.IsDead) {
            IsTouchingMap = false;
            standingOnJumpPad = false;
            return;
        }

        if (Input.GetKeyDown(KeyCode.W))
            jumpInput = true;

        if (Input.GetKeyDown(KeyCode.S))
            downThrustInput = true;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        //use A and D keys for left or right movement
        DirX = 0;
        if (Input.GetKey(KeyCode.D))
            DirX++;
        if (Input.GetKey(KeyCode.A))
            DirX--;

        if (downThrustInput && !isThrustingDownwards && phaseTracker.IsMidAir) {
            downThrustInput = false;
            rig.velocity = new Vector2(rig.velocity.x, 0);
            rig.AddForce(new Vector2(0, DOWNWARDS_THRUST_FORCE));
            isThrustingDownwards = true;
            fastestYVelocityRecorded = 0;
        }

        setPlayerVelocity();

        //use W and S keys for jumping up or thrusting downwards + allow double jump
        if (jumpInput) {
            jumpInput = false;
            if (IsGrounded && !phaseTracker.Is(Phase.Jumping) && !standingOnJumpPad)
                StartCoroutine(normalJump());

            else if (phaseTracker.Is(Phase.Jumping))
                StartCoroutine(doubleJump());

            else if (IsGrounded && !phaseTracker.Is(Phase.Jumping) && standingOnJumpPad)
                StartCoroutine(jumpPadBoost());
        }
    }

    private void setPlayerVelocity()
    {
        //nullify the slight bounce on a slope glitch when changing slopes
        //if ((!phaseTracker.IsMidAir || phaseTracker.Is(Phase.Falling)) && rig.velocity.y > 0)
        //  rig.velocity = new Vector2(0, 0);

        if (wallInFrontOfYou && ((DirX == 1 && lookAround.IsFacingRight) || (DirX == -1 && !lookAround.IsFacingRight))) {
            setVelocity(new Vector2(0, rig.velocity.y));
            rig.gravityScale = Game.GRAVITY;
        } else if (wallBehindYou && ((DirX == 1 && !lookAround.IsFacingRight) || (DirX == -1 && lookAround.IsFacingRight))) {
            setVelocity(new Vector2(0, rig.velocity.y));
            rig.gravityScale = Game.GRAVITY;
        }

          //when player is on the ground, player velocity is parallel to the slanted ground 
          else if (!phaseTracker.IsMidAir && IsGrounded) {
            if (IsOnSuperSteepSlope) {
                int dirX;
                if (wallInFrontOfYou && lookAround.IsFacingRight || wallBehindYou && !lookAround.IsFacingRight)
                    dirX = Math.Min(DirX, 0);
                else
                    dirX = Math.Max(DirX, 0);


                if (dirX == 0) {
                    // Vector2 slideDownSlopeDirection = Mathf.Sign(groundSlope.y) * groundSlope;
                    // rig.gravityScale = GRAVITY * slideDownSlopeDirection.normalized.y;
                    rig.gravityScale = Game.GRAVITY;
                } else {
                    setVelocity(new Vector2(speed * DirX, rig.velocity.y));
                    rig.gravityScale = Game.GRAVITY;
                }
            } else {
                float speedMultiplier = phaseTracker.IsWalkingBackwards ? 0.87f : 1f;
                setVelocity(DirX * speed * speedMultiplier * groundSlope);
                rig.gravityScale = (DirX == 0) ? 0f : Game.GRAVITY;
            }

            //camera shakes if landing from a downwards thrust
            if (isThrustingDownwards && fastestYVelocityRecorded < -20f) {
                float shakeMultiplier = 0.4f + (-fastestYVelocityRecorded - 40f) / 100f;
                cameraMovement.ExecuteCameraShake(shakeMultiplier);
            }


            //allow player to thrust themselves downwards the next time they jump
            isThrustingDownwards = false;
        } else {
            setVelocity(new Vector2(speed * DirX, rig.velocity.y));
            rig.gravityScale = Game.GRAVITY;
        }

        if (rig.velocity.y < fastestYVelocityRecorded)
            fastestYVelocityRecorded = rig.velocity.y;
    }

    private void setVelocity(Vector2 velocity)
    {
        rig.AddForce((velocity * rig.mass - rig.velocity * rig.mass) * MOVEMENT_ALTERATION_SPEED);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.JumpPad)
            standingOnJumpPad = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.JumpPad)
            standingOnJumpPad = false;
    }

    private HashSet<GameObject> wallsInFront = new HashSet<GameObject>();
    private HashSet<GameObject> wallsBehind = new HashSet<GameObject>();

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform) {
            ContactPoint2D[] contacts = new ContactPoint2D[5];
            int numOfContactsDetected = col.GetContacts(contacts);

            for (int i = 0; i < numOfContactsDetected; i++) {
                // wall in front if it has a small y normal and you're facing it
                if (contacts[i].normal.y < maxYNormalOfWalls &&
                    (lookAround.IsFacingRight && contacts[i].point.x > transform.position.x || !lookAround.IsFacingRight && contacts[i].point.x < transform.position.x)) ;
                wallsInFront.Add(col.gameObject);

                if (contacts[i].normal.y < maxYNormalOfWalls &&
                   (lookAround.IsFacingRight && contacts[i].point.x < transform.position.x || !lookAround.IsFacingRight && contacts[i].point.x > transform.position.x)) ;
                wallsBehind.Add(col.gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform) {
            ContactPoint2D[] contacts = new ContactPoint2D[5];
            int numOfContactsDetected = col.GetContacts(contacts);

            for (int i = 0; i < numOfContactsDetected; i++) {
                // wall in front if it has a small y normal and you're facing it
                if (contacts[i].normal.y < maxYNormalOfWalls &&
                    (lookAround.IsFacingRight && contacts[i].point.x > transform.position.x || !lookAround.IsFacingRight && contacts[i].point.x < transform.position.x)) ;
                wallsInFront.Add(col.gameObject);

                if (contacts[i].normal.y < maxYNormalOfWalls &&
                   (lookAround.IsFacingRight && contacts[i].point.x < transform.position.x || !lookAround.IsFacingRight && contacts[i].point.x > transform.position.x)) ;
                wallsBehind.Add(col.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.layer == Layer.DefaultPlatform || col.gameObject.layer == Layer.OneSidedPlatform) {
            wallsInFront.Remove(col.gameObject);
            wallsBehind.Remove(col.gameObject);
        }
    }
}
