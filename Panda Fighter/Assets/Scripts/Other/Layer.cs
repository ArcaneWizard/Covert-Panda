
using System.Collections.Generic;
using UnityEngine;

public static class Layer
{
    public static int ArmorOrLimb = 5;
    public static int ArmorOrLimbInRagdoll = 0;

    public static int DefaultGround = 11;

    // layer for the creature's one way collider (helps creature to jump onto bridge, but not fall through a bridge)
    public static int OneWayCollider = 4;
    public static int OneWayGround = 20;

    public static int Weapons = 15;
    public static int JumpPad = 19;

    public static int FriendlyBullet = 13;
    public static int EnemyBullet = 17;
    public static int Explosion = 10;

    public static int FriendlyHitBox = 14;
    public static int EnemyHitBox = 6;
    public static int InvulnerableHitBox = 1;

    public static int Friend = 12;
    public static int Enemy = 9;

    // returns a layer for the hit box of the opposite side's creatures 
    public static int GetHitBoxOfOpposition(Side side) =>  
        (side == Side.Friendly) ? EnemyHitBox : FriendlyHitBox;
    
    // returns a layer for the hit box of whichever side this bullet is intended to hit
    public static int GetHitBoxOfOpposition(GameObject bullet) =>
        (bullet.layer == FriendlyBullet) ? EnemyHitBox : FriendlyHitBox;
}
