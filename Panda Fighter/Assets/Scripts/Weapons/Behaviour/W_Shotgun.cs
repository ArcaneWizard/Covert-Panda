using UnityEngine;

public class W_Shotgun : WeaponBehaviour
{
    private static Vector2 bulletSpawnOffsetRange = new Vector2(0.1f, 0.4f);
    private static Vector2 bulletSpreadAngleRange = new Vector2(2f, 8f);

    protected override void attack(Vector2 aim)
    {
        float bulletSpread = Random.Range(bulletSpreadAngleRange.x, bulletSpreadAngleRange.y);

        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);

        CommonWeaponBehaviours.SpawnAndShootBulletDiagonally(aim, bulletSpread, bulletSpawnOffsetRange,
             weaponSystem, weaponConfiguration, side);

        CommonWeaponBehaviours.SpawnAndShootBulletDiagonally(aim, -bulletSpread, bulletSpawnOffsetRange,
             weaponSystem, weaponConfiguration, side);
    }
}
