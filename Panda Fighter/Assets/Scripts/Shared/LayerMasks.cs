
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public static class LayerMasks
{
    // returns a layermask allowing collisions with the map
    public static LayerMask map => 1 << Layers.map;

    // returns a layermask allowing collisions with the map and the hit box of the opposite side (friends or enemies)
    public static LayerMask mapOrTarget(Side side) => map | target(side);
    public static LayerMask mapOrTarget(Transform bullet) => map |
        ((bullet.gameObject.layer == Layers.friendlyBullet) ? (1 << Layers.enemyHitBox) : (1 << Layers.friendlyHitBox));

    // returns a layermask allowing collisions with the hit box of the opposite entity 
    // (player/friendly AI hit boxes for enemies and enemiy hit boxes for the player/friendly bots)
    private static LayerMask target(Side side)
    {
        return (side == Side.Friendly)
            ? (1 << Layers.enemyHitBox)
            : (1 << Layers.friendlyHitBox);
    }

}
