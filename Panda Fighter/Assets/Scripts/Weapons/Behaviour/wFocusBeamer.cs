using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFocusBeamer : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponAction.SpawnBullet(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.GetComponent<FocusBeam>().Beam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        confirmAttackFinished();
        yield return null;
    }

}
