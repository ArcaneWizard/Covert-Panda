using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : WeaponImplementation
{
    public override IEnumerator StartAttack(Vector2 aim)
    {
        StartCoroutine(base.StartAttack(aim));

        WeaponAction.ShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);

        FinishAttack();
        yield return null;
    }

}
