using UnityEngine;

public class W_PlasmaOrb : WeaponBehaviour
{
    private static Vector2 forceMultiplier = new Vector2(1.4f, 1.5f);
    private static Vector2 forceOffset = new Vector2(0f, 0f);

    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
             weaponSystem, weaponConfiguration, side);

        AttackProgress = AttackProgress.Finished;
    }
}