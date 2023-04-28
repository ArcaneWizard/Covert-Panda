using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFocusBeamer : WeaponBehaviour
{
    protected override void startAttack()
    {
        PhysicalBullet bullet = WeaponBehaviourHelper.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);

        bullet.transform.GetComponent<FocusBeam>().ShootBeam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        bullet.transform.GetComponent<Bullet>().StartCollisionDetection(aim,
            BulletMovementAfterFiring.SyncedWithDirectionAimedIn, false);
    }
}
