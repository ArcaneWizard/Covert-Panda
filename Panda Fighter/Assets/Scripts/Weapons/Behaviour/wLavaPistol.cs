using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wLavaPistol : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.localEulerAngles = Vector3.zero;

        confirmAttackFinished();
        yield return null;
    }

}
