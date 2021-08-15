using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : IWeapon {
    
    protected float orbThrowForce = 2200;
    protected float orbYForce = -20; 
    
    public override void SetDefaultAnimation() =>  config.animator.SetInteger("Arms Phase", 0);

  public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        config.animator.SetInteger("Arms Phase", 1);
        float wait = reusableWeaponMethods.calculateTimeB4ReleasingWeapon(0.1f, 0.22f, aim);

        yield return new WaitForSeconds(wait);
        DoAttack(aim, bullet, rig);
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig) {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);
        bullet.transform.right = aim;

        Vector2 unadjustedForce = orbThrowForce * aim * new Vector2(1.2f, 1) + new Vector2(0, orbYForce);
        bulletRig.AddForce(unadjustedForce * bulletRig.mass);
    }
}
