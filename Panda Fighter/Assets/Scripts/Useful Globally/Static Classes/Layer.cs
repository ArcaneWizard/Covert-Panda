
using System.Collections.Generic;
using UnityEngine;

public static class Layer
{
    // damaging stuff
    public static int FriendlyBullet = 13;
    public static int EnemyBullet = 17;
    public static int Explosion = 10;

    // hitboxes
    public static int FriendlyHitBox = 14;
    public static int EnemyHitBox = 6;
    public static int InvulnerableHitBox = 1;

    // limbs and armor
    public static int LimbsAndArmor = 5;
    public static int LimbInRagdoll = 0;

    // map and colliders
    public static int DefaultPlatform = 11;
    public static int OneSidedPlatform = 20;

    /// <summary> Collides with default platforms. Also detects weapons, jump pads, and ai decision zones </summary>
    public static int DefaultCollider = 0;
    public static int OneSidedPlatformCollider = 4;

    // other
    public static int PickableWeapons = 15;
    public static int AiDecisionZone = 8;
    public static int JumpPad = 19;

    // DEPRECATED (LAYERS NOT BEING USED IN GAME)
    public static int Friend = 12;
    public static int Enemy = 9;

    // useful functions
    public static int GetHitBoxOfOpposingSide(Side side) =>  
        (side == Side.Friendly) ? EnemyHitBox : FriendlyHitBox;

    /// <summary> Returns a layer allowing collisions against whichever side this bullet 
    /// is intended to hit </summary>
    public static int GetHitBoxOfOpposingSide(GameObject bullet) =>
        (bullet.layer == FriendlyBullet) ? EnemyHitBox : FriendlyHitBox;
}
