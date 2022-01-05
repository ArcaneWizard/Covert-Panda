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
        reusableWeaponMethods.configureReusedBullet(bullet, rig, configuration.bulletSpawnPoint, side);

        bullet.transform.right = aim;
        Vector2 unadjustedForce = configuration.bulletSpeed * 40 * aim * new Vector2(1.4f, 1);
        rig.AddForce(unadjustedForce * rig.mass);
    }
}
