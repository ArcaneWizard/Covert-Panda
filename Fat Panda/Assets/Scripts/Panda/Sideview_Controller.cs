using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sideview_Controller : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer sR;

    public float speed = 2.0f;
    public float jumpForce = 200;
    private int movementDirX;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        //convert A and D keys to left or right movement
        movementDirX = 0;
        if (Input.GetKey(KeyCode.D))
            movementDirX++;
        if (Input.GetKey(KeyCode.A))
            movementDirX--;

        rig.velocity = new Vector2(speed * movementDirX, rig.velocity.y);

        //convert W and S keys to jumping up or thrusting downwards
        if (Input.GetKeyDown(KeyCode.W))
            rig.AddForce(new Vector2(0, jumpForce));
        if (Input.GetKeyDown(KeyCode.S))
            rig.AddForce(new Vector2(0, -jumpForce));

        //orient the player to face the same direction they're moving in
        playerOrientation();
    }

    private void playerOrientation()
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }
}
