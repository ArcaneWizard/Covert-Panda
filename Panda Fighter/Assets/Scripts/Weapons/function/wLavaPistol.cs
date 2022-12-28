using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wLavaPistol : WeaponImplementation
{
    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint, side);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, config.bulletSpeed);

        bullet.localEulerAngles = new Vector3(0, 0, 0);
    }
}
