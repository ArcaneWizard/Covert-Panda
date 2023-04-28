using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
        bullet.transform.GetComponent<Bullet>().OnFire(aim, BulletMovementAfterFiring.StraightLine, false);

        confirmAttackFinished();
        yield return null;
    }

}
