using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wLavaPistol : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        Transform bullet = WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.localEulerAngles = Vector3.zero;

        ConfirmAttackFinished();
        yield return null;
    }

}
