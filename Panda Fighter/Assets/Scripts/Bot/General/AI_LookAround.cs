using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_LookAround : CentralLookAround
{
    public Transform player;
    public LayerMask mapOrPlayer;
    public bool playerIsInSight { get; private set; }

    public Vector3 lookAt { get; private set; }
    private Vector3 pretendCursor = new Vector3(0, 0, 0), updatedCursorPos;
    private float cursorMovementSpeed = 0.3f, cursorVelX, cursorVelY;
    private float cursorX, cursorY;

    private int[] signs = new int[3];
    private System.Random random;

    public override void Awake()
    {
        base.Awake();

        random = new System.Random();
        pretendCursor = newCursorPosition();

        StartCoroutine(pretendCursorMovement());
        StartCoroutine(IsPlayerInLineOfSight());
    }

    private void LateUpdate()
    {
        lookAt = (playerIsInSight)
            ? player.position - shootingArm.position
            : new Vector3(shooting.getAim().x, shooting.getAim().y, 0);

        lookAndAimInRightDirection();

        if (pretendCursor != updatedCursorPos)
        {
            cursorX = Mathf.SmoothDamp(pretendCursor.x, updatedCursorPos.x, ref cursorVelX, 1f / cursorMovementSpeed);
            cursorY = Mathf.SmoothDamp(pretendCursor.y, updatedCursorPos.y, ref cursorVelY, 1f / cursorMovementSpeed);
            pretendCursor = new Vector3(cursorX, cursorY, pretendCursor.z);
        }
    }

    //handles player orientation (left/right), gun rotation, gun position, head rotation
    private void lookAndAimInRightDirection()
    {
        //if player isn't spinning in mid-air with a double jump
        if (!animController.disableLimbsDuringDoubleJump)
        {
            //player faces left or right depending on mouse cursor
            body.localRotation = (lookAt.x > 0) ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);

            //calculate the angle btwn mouse cursor and player's shooting arm
            Vector2 shootDirection = lookAt;
            float shootAngle = Mathf.Atan2(shootDirection.y, Mathf.Abs(shootDirection.x)) * 180 / Mathf.PI;

            //apply offset to the shoot Angle when the player is tilted on a ramp:
            float zAngle = ((180 - Mathf.Abs(180 - transform.eulerAngles.z))); // <- maps angles above 180 to their negative value instead (ex. 330 becomes -30)
            zAngle *= (body.localEulerAngles.y / 90 - 1) * Mathf.Sign(transform.eulerAngles.z - 180);
            shootAngle -= zAngle;

            base.rotateHeadAndWeapon(shootDirection, shootAngle);
        }
    }

    private Vector3 newCursorPosition()
    {
        signs = lookAt != null && lookAt.x > 0 ? new int[] { -1, -1, 1 } : new int[] { -1, 1, 1 };
        return new Vector3(7f * signs[random.Next(0, signs.Length)], random.Next(-8, 9), 0);
    }

    private IEnumerator pretendCursorMovement()
    {
        updatedCursorPos = newCursorPosition();
        cursorMovementSpeed = random.Next(17, 31) / 10f;

        yield return new WaitForSeconds(random.Next(12, 30) / 10f);
        StartCoroutine(pretendCursorMovement());
    }

    private IEnumerator IsPlayerInLineOfSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(shootingArm.position, player.position - shootingArm.position, 50, mapOrPlayer);
        playerIsInSight = hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player");

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(IsPlayerInLineOfSight());
    }
}
