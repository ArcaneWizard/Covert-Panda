using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wLavaPistol : IWeapon
{
    private LayerMask map = 1 << 11;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, config.bulletSpeed);

        bullet.localEulerAngles = new Vector3(0, 0, 0);
        RaycastHit2D hit = Physics2D.Raycast(config.bulletSpawnPoint.position, aim, 80f, map);
        bullet.transform.GetComponent<LavaOrb>().OrientExplosion(hit.normal);
    }
}
