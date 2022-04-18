
using System.Collections.Generic;
using UnityEngine;

public static class Layers
{
    // layer for the creature's armor/limbs when they are dead and ragdolling
    public static int deadRagdoll = 0;

    // layer for the creature's armor/limbs when they are alive
    public static int detectPickableWeapons = 5;

    // layer for the creature's hit box when they are invulnerable
    public static int collideWithNothing = 1;

    // layer for the creature's one way collider (goes through one-way-ground when jumping, otherwise walks on it like normal ground)
    public static int oneWayCollider = 4;

    public static int ground = 11;
    public static int oneWayGround = 20;
    public static int weapons = 15;
    public static int jumpPad = 19;

    public static int friendlyBullet = 13;
    public static int enemyBullet = 17;
    public static int explosion = 10;

    public static int friendlyHitBox = 14;
    public static int enemyHitBox = 6;

    public static int friend = 12;
    public static int enemy = 9;


    // returns a layer allowing collisions with the hit box of the opposite entity 
    // (player/friendly AI hit boxes for enemies and enemy hit boxes for the player/friendly bots)
    public static int target(Side side) =>  
        (side == Side.Friendly) ? enemyHitBox : friendlyHitBox;
    
    // returns a layer allowing collisions with whichever side this bullet is intended tohit
    
    public static LayerMask target(GameObject bullet) =>
        (bullet.layer == friendlyBullet) ? enemyHitBox : friendlyHitBox;
}
