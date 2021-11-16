using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : IWeapon
{

    private float bulletSpeed = 72;

    public override void Awake() { base.Awake(); config.IK_Coordinates = AimingDir.arcticPistolAiming; }

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, rig, bulletSpeed);
    }
}
