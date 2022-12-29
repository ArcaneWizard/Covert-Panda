
using System.Collections.Generic;
using UnityEngine;

public static class Layer
{

    // bullets and explosions
    public static int FriendlyBullet = 13;
    public static int EnemyBullet = 17;
    public static int Explosion = 10;

    // hitboxes which detect bullets or explosions
    public static int FriendlyHitBox = 14;
    public static int EnemyHitBox = 6;
    public static int InvulnerableHitBox = 1;

    // exact "outline" of a creature 
    public static int ArmorOrLimb = 5;
    public static int ArmorOrLimbInRagdoll = 0;

    // any part of the map (ex. platforms)
    public static int DefaultGround = 11;

    // one way ground is s part of the map that only gets detected by the creature's one-way collider and that too 
    // when the creature isn't jumping. An example is a bridge that a creature can jump onto, but not fall down
    // when walking on it.
    public static int OneWayCollider = 4;
    public static int OneWayGround = 20;

    public static int Weapons = 15;
    public static int JumpPad = 19;

    public static int Friend = 12;
    public static int Enemy = 9;

    // returns a layer for the hit box of the opposite side's creatures 
    public static int GetHitBoxOfOpposition(Side side) =>  
        (side == Side.Friendly) ? EnemyHitBox : FriendlyHitBox;
    
    // returns a layer for the hit box of whichever side this bullet is intended to hit
    public static int GetHitBoxOfOpposition(GameObject bullet) =>
        (bullet.layer == FriendlyBullet) ? EnemyHitBox : FriendlyHitBox;
}
