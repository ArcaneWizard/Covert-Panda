using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : IWeapon
{

    protected float orbThrowForce = 2000;
    protected float orbYForce = 0;

    public override void Awake() { base.Awake(); config.IK_Coordinates = AimingDir.boomerangAiming; }

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);

        bullet.transform.right = aim;
        Vector2 unadjustedForce = orbThrowForce * aim * new Vector2(1.2f, 1) + new Vector2(0, orbYForce);
        rig.AddForce(unadjustedForce * rig.mass);
    }
}
