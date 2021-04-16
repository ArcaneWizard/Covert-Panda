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
    public void jump(Rigidbody2D rig, float jumpForce, bool boost)
    {
        float xForce = (boost) ? rig.velocity.x * 10 : 0;
        rig.AddForce(new Vector2(xForce, jumpForce));
    }

    //check if there is a floor gap or wall or ceiling gap nearby  
    public WallChecker wallChecks(Transform leftFoot, Transform rightFoot, Transform leftHead, Transform rightHead, LayerMask map, Transform bot)
    {
        WallChecker info = new WallChecker(false, null, null);

        RaycastHit2D leftWall = Physics2D.Raycast(bot.position, Vector2.left, 0.4f, map);
        RaycastHit2D rightWall = Physics2D.Raycast(bot.position, Vector2.right, 0.4f, map);

        RaycastHit2D leftCeiling = Physics2D.Raycast(leftHead.position, Vector2.up, 3f, map);
        RaycastHit2D rightCeiling = Physics2D.Raycast(rightHead.position, Vector2.up, 3f, map);

        RaycastHit2D leftGround = Physics2D.Raycast(leftFoot.position, Vector2.down, 3f, map);
        RaycastHit2D rightGround = Physics2D.Raycast(rightFoot.position, Vector2.down, 3f, map);

        //if the bot is super close to a wall, it needs to figure out what to do
        if ((leftWall.collider || rightWall.collider)
            && (leftGround.collider && rightGround.collider))
        {
            info.wallNearby = true;
        }

        //check whether the bot has a ground opening to its left and right
        if (leftGround.collider && !rightGround.collider)
            info.floorOpening = "right";
        else if (!leftGround.collider && rightGround.collider)
            info.floorOpening = "left";
        else if (!leftGround.collider && !rightGround.collider)
            info.floorOpening = "both";
        else
            info.floorOpening = "none";

        //check whether the bot has ceiling to its left and right
        if (leftCeiling.collider && !rightCeiling.collider)
            info.ceilingOpening = "right";
        else if (!leftCeiling.collider && rightCeiling.collider)
            info.ceilingOpening = "left";
        else if (!leftCeiling.collider && rightCeiling.collider)
            info.ceilingOpening = "both";
        else
            info.ceilingOpening = "none";

        Debug.LogFormat("Wall nearby: {0}, Floor: {1}, Ceiling: {2}", info.wallNearby, info.floorOpening, info.ceilingOpening);

        return info;
    }



}
