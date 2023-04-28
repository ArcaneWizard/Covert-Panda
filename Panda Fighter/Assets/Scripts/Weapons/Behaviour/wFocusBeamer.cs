using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFocusBeamer : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        PhysicalBullet bullet = WeaponBehaviourHelper.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);
        bullet.transform.GetComponent<FocusBeam>().ShootBeam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        bullet.transform.GetComponent<Bullet>().OnFire(aim, BulletMovementAfterFiring.SyncedWithDirectionAimedIn, false);

        confirmAttackFinished();
        yield return null;
    }
}
