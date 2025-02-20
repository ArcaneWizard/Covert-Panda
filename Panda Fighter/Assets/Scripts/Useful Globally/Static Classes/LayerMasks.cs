using UnityEngine;

public static class LayerMasks
{
    public static LayerMask Map => 1 << Layer.DefaultPlatform | 1 << Layer.OneSidedPlatform;

    ///<summary> allows collisions with creatures on the opposite of the specified side </summary> 
    public static LayerMask Target(Side side) =>
        (side == Side.Friendly) ? (1 << Layer.EnemyHitBox) : (1 << Layer.FriendlyHitBox);

    ///<summary> allows collisions with creatures the bullet is allowed to hit </summary> 
    public static LayerMask Target(Transform bullet)
    {
        return (bullet.gameObject.layer == Layer.FriendlyBullet)
            ? (1 << Layer.EnemyHitBox) : (1 << Layer.FriendlyHitBox);
    }

    ///<summary> allows collisions with the map or creatures on the opposite side </summary>
    public static LayerMask MapOrTarget(Side side) => Map | Target(side);

    ///<summary> allows collisions with the map or creatures this bullet is allowed to hit </summary>
    public static LayerMask MapOrTarget(Transform bullet) => Map | Target(bullet);

    public static bool ContainsLayer(this LayerMask mask, int layer) => ((mask.value >> layer) & 1) == 1;
}
