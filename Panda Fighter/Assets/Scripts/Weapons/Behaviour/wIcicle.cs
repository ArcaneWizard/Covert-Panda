using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        WeaponAction.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side, false);

        confirmAttackFinished();
        yield return null;
    }

}
