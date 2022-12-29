using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : WeaponImplementation
{
    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        WeaponAction.ConfigureBullet(bullet, rig, weaponConfiguration.bulletSpawnPoint, side);
        WeaponAction.ShootBulletInArc(aim, bullet, rig, new Vector2(1.4f, 1.5f), weaponConfiguration.bulletSpeed, true);
    }
}
