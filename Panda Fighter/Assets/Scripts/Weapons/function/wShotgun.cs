using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShotgun : WeaponImplementation
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
        ReusableWeaponImplentations.ConfigureBullet(bullet, bulletRig, config.bulletSpawnPoint, side);
        ReusableWeaponImplentations.ShootBullet(aim, bullet, bulletRig, config.bulletSpeed);

        bulletSpread = UnityEngine.Random.Range(2, 8f);
        ReusableWeaponImplentations.configureNewBulletAndShootAtAngle(bulletSpread, aim, config, side, bulletSpawnOffset);

        bulletSpread = UnityEngine.Random.Range(2, 8f);
        ReusableWeaponImplentations.configureNewBulletAndShootAtAngle(-bulletSpread, aim, config, side, bulletSpawnOffset);
    }
}
