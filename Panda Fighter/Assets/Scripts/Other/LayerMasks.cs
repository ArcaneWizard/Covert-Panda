
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public static class LayerMasks
{
    // returns a layermask allowing collisions with the map
    public static LayerMask map => 1 << Layer.DefaultGround | 1 << Layer.OneWayGround;

    // returns a layermask allowing collisions with the map and the hit box of the opposite side (friends or enemies)
    public static LayerMask mapOrTarget(Side side) => map | target(side);
    public static LayerMask mapOrTarget(Transform bullet) => map |
        ((bullet.gameObject.layer == Layer.FriendlyBullet) ? (1 << Layer.EnemyHitBox) : (1 << Layer.FriendlyHitBox));

    // returns a layermask allowing collisions with the hit box of the opposite entity 
    // (player/friendly AI hit boxes for enemies and enemiy hit boxes for the player/friendly bots)
    public static LayerMask target(Side side)
    {
        return (side == Side.Friendly)
            ? (1 << Layer.EnemyHitBox)
            : (1 << Layer.FriendlyHitBox);
    }

}
