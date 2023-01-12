using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShotgun : WeaponBehaviour
{
    private static Vector2 bulletSpawnOffsetRange = new Vector2(0.1f, 0.4f);
    private static Vector2 bulletSpreadAngleRange = new Vector2(2f, 8f);

    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        float bulletSpread = Random.Range(bulletSpreadAngleRange.x, bulletSpreadAngleRange.y);

        WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);

        WeaponAction.SpawnAndShootBulletDiagonally(aim, bulletSpread, bulletSpawnOffsetRange,
             weaponSystem, weaponConfiguration, side, false);

        WeaponAction.SpawnAndShootBulletDiagonally(aim, -bulletSpread, bulletSpawnOffsetRange,
             weaponSystem, weaponConfiguration, side, false);

        ConfirmAttackFinished();
        yield return null;
    }
}
