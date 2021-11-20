using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wGrenade : IWeapon
{

    protected float grenadeThrowForce = 2200;
    protected float grenadeYForce = -20;

    public override void SetDefaultAnimation() => config.animator.SetInteger("Arms Phase", 0);

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        config.animator.SetInteger("Arms Phase", 1);

        float wait = reusableWeaponMethods.calculateTimeB4ReleasingWeapon(0.02f, 0.0f, aim);
        yield return new WaitForSeconds(wait);
        DoAttack(aim, bullet, rig);
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);
        bullet.transform.right = -aim;

        bullet.GetComponent<Collider2D>().isTrigger = false;
        bullet.transform.GetComponent<Grenade>().startExplosionTimer();

        Vector2 unadjustedForce = config.bulletSpeed * 40 * aim * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        bulletRig.AddForce(unadjustedForce * bulletRig.mass);
    }
}
