using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : IWeapon
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
        bullet.transform.GetComponent<SniperBeam>().ShowBeam();
    }
}
