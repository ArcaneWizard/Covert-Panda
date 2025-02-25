using UnityEngine;

public class W_ArcticSprayer : WeaponBehaviour
{
    private static Vector2 forceMultiplier = new Vector2(1.0f, 1.1f);
    private static Vector2 forceOffset = Vector2.zero;

    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset, weaponSystem, weaponConfiguration, side);
        AttackProgress = AttackProgress.Finished;
    }
}
