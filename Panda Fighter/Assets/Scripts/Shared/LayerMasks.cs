
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public static class LayerMasks
{
    // returns a layermask allowing collisions with the map and the hit box of the opposite entity 
    // (player's/friendly AI's hit boxes for enemies and enemies' hit box for the player/friendly bots)
    public static LayerMask mapOrTarget(Transform bullet)
    {
        return (bullet.gameObject.layer == Layers.friendlyBullet)
            ? (1 << Layers.map | 1 << Layers.enemyHitBox)
            : (1 << Layers.map | 1 << Layers.friendlyHitBox);
    }

    public static LayerMask map => 1 << Layers.map;
}
