using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShotgun : WeaponMechanics
{
    private Vector2 bulletSpawnOffset = new Vector2(0.1f, 0.4f);
    private float bulletSpread;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint, side);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, bulletRig, config.bulletSpeed);

        bulletSpread = UnityEngine.Random.Range(2, 8f);
        reusableWeaponMethods.configureNewBulletAndShootAtAngle(bulletSpread, aim, config, side, bulletSpawnOffset);

        bulletSpread = UnityEngine.Random.Range(2, 8f);
        reusableWeaponMethods.configureNewBulletAndShootAtAngle(-bulletSpread, aim, config, side, bulletSpawnOffset);
    }
}
