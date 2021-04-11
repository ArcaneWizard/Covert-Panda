using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator animator;
    private SpriteRenderer sR;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetChild(0).transform.GetComponent<Animator>();
        sR = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();

        Debug.DrawLine(transform.position, new Vector2(transform.position.x - 10, transform.position.y), Color.red, 10f, false);
    }

    void Update()
    {
        //orient the player to face the same direction they're moving in
        playerOrientation();

        //set idle or move animations
        playerAnimation();
    }

    private void playerAnimation()
    {
        if (Mathf.Abs(rig.velocity.x) > 0)
            animator.SetInteger("State", 1);
        else
            animator.SetInteger("State", 0);
    }

    private void playerOrientation()
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }
}
