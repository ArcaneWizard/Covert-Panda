using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : IWeapon
{
    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);

        bullet.transform.right = aim;
        Vector2 unadjustedForce = config.bulletSpeed * 40 * aim * new Vector2(1.2f, 1);
        rig.AddForce(unadjustedForce * rig.mass);
    }
}
