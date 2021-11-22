using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wNeedler : IWeapon
{

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, configuration.bulletSpawnPoint);
        bullet.position += new Vector3(0, UnityEngine.Random.Range(-0.13f, 0.13f), 0f);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, configuration.bulletSpeed);
    }
}
