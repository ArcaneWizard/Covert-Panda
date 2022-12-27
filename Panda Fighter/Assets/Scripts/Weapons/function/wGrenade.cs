using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wGrenade : WeaponMechanics
{
    protected float grenadeThrowForce = 2200;
    protected float grenadeYForce = -20;

    public override void SetDefaultAnimation() => config.animator.SetInteger("Arms Phase", 0);

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        config.limbs[1].transform.parent.GetComponent<Animator>().Play(0, -1);
        config.limbs[1].SetActive(true);

        float wait = reusableWeaponMethods.calculateTimeB4ReleasingWeapon(0.02f, 0.2f, aim);
        yield return new WaitForSeconds(wait);
        DoAttack(aim, bullet, rig);

        yield return new WaitForSeconds(0.6f-wait);
        config.limbs[1].SetActive(false);
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint, side);
        bullet.transform.right = -aim;

        bullet.GetComponent<Collider2D>().isTrigger = false;
        bullet.transform.GetComponent<Grenade>().startExplosionTimer();

        Vector2 unadjustedForce = config.bulletSpeed * 40 * aim * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        bulletRig.AddForce(unadjustedForce * bulletRig.mass);
    }
}
