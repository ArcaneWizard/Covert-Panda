using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);

        ConfirmAttackFinished();
        yield return null;
    }

}
