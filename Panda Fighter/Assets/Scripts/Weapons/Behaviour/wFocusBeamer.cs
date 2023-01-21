using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFocusBeamer : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        Transform bullet = WeaponAction.SpawnBullet(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.GetComponent<FocusBeam>().Beam(weaponConfiguration.BulletSpawnPoint,
            weaponConfiguration.PhysicalWeapon.transform, phaseTracker.IsDoingSomersault);

        ConfirmAttackFinished();
        yield return null;
    }

}
