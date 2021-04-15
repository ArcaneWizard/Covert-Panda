using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedBotBehaviour 
{

    //set bot's idle or move animation
    public void botAnimation(Animator anim, Rigidbody2D rig)
    {
        if (Mathf.Abs(rig.velocity.x) > 0.6f)
            anim.SetInteger("State", 1);
        else
            anim.SetInteger("State", 0);
    }

    //orient the bot to face the direction they're moving in
    public void botOrientation(Rigidbody2D rig, SpriteRenderer sR)
    {
        if (rig.velocity.x > 0)
            sR.flipX = true;
        else if (rig.velocity.x < 0)
            sR.flipX = false;
    }

    //bot jumps
    public void jump(Rigidbody2D rig, float jumpForce)
    {
        rig.AddForce(new Vector2(rig.velocity.x * 10, jumpForce));
    }

    //check if there still ground/a surface to walk on nearby + check if there a nearby wall 
    public Vector2 wallChecks(Transform leftFoot, Transform rightFoot, LayerMask map, Transform bot)
    {
        Vector2 info = new Vector2(0, 0);

        RaycastHit2D leftWall = Physics2D.Raycast(bot.position, Vector2.left, 0.4f, map);
        RaycastHit2D rightWall = Physics2D.Raycast(bot.position, Vector2.right, 0.4f, map);

        RaycastHit2D leftGround = Physics2D.Raycast(leftFoot.position, Vector2.down, 2f, map);
        RaycastHit2D rightGround = Physics2D.Raycast(rightFoot.position, Vector2.down, 2f, map);

        //if the bot is super close to a wall, it needs to figure out what to do
        if ((leftWall.collider != null || rightWall.collider != null)
            && (leftGround.collider != null && rightGround.collider != null))
        {
            info.x = 1;
        }


        //check whether the bot has ground to its left and right
        if (leftGround.collider != null && rightGround.collider == null)
            info.y = -1;
        else if (leftGround.collider == null && rightGround.collider != null)
            info.y = 1;
        else
            info.y = 0;

        return info;
    }
}
