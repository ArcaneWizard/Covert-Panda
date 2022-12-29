using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wLavaPistol : WeaponImplementation
{
    public override IEnumerator StartAttack(Vector2 aim)
    {
        StartCoroutine(base.StartAttack(aim));

        Transform bullet = WeaponAction.ShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.localEulerAngles = Vector3.zero;

        FinishAttack();
        yield return null;
    }

}
