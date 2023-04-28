using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class wLavaPistol : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);

        bullet.localEulerAngles = Vector3.zero;
        bullet.transform.GetComponent<Bullet>().OnFire(aim, BulletMovementAfterFiring.StraightLine, false);

        confirmAttackFinished();
        yield return null;
    }
}
