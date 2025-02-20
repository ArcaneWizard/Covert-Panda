using UnityEngine;

public class W_NormalWeapon : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
    }
}
