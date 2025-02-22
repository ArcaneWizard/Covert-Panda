using UnityEngine;

public class W_Icicle : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);

        AttackProgress = AttackProgress.Finished;
    }
}
