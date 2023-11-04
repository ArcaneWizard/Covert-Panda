using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFocusBeamer : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        PhysicalBullet bullet = CommonWeaponBehaviours.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);

        bullet.transform.GetComponent<FocusBeam>().ShootBeam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        bullet.transform.GetComponent<Bullet>().OnFire(aim);
    }
}