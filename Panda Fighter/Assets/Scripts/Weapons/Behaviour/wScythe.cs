using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wScythe : WeaponBehaviour
{
    public override void UponSwitchingToWeapon()
    {
        base.UponSwitchingToWeapon();
        weaponConfiguration.Animator.SetInteger("Arms Phase", 10);
    }

    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        weaponConfiguration.Animator.SetInteger("Arms Phase", 11);
        weaponConfiguration.MainArmIKTracker.gameObject.SetActive(false);

        while (weaponConfiguration.Animator.GetInteger("Arms Phase") == 11)
            yield return null;

        confirmAttackFinished();
    }

   /* public override IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        float throwDelay = reusableWeaponMethods.calculateTimeB4ReleasingWeapon(0.04f, 0.22f, aim);
        config.animator.SetInteger("Arms Phase", 11);

        yield return new WaitForSeconds(throwDelay);
        DoBonusAttack(aim, bullet, bulletRig);
    }

    public override void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        bullet.transform.position = config.weapon.transform.position;
        bullet.transform.localEulerAngles = config.weapon.transform.localEulerAngles;
        bullet.transform.localScale = config.weapon.transform.localScale;
        config.weapon.gameObject.SetActive(false);

        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.weapon.transform, side);
        bulletRig.velocity = aim * config.bulletSpeed;
        bulletRig.angularVelocity = Random.Range(scytheSpinSpeed.x, scytheSpinSpeed.y) * Mathf.Sign(-aim.x);
    }*/
}

