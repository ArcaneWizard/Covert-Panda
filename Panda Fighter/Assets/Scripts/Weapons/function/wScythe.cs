using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wScythe : IWeapon
{
    private Vector2 scytheSpinSpeed = new Vector2(1200, 1400);

    public override void SetDefaultAnimation() => configuration.animator.SetInteger("Arms Phase", 10);

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        configuration.animator.SetInteger("Arms Phase", 11);
        configuration.aimTarget.gameObject.SetActive(false);

        while (configuration.animator.GetInteger("Arms Phase") == 11)
            yield return null;

        DoAttack(aim, bullet, rig);
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig) => configuration.aimTarget.gameObject.SetActive(true);

    public override IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        float throwDelay = reusableWeaponMethods.calculateTimeB4ReleasingWeapon(0.04f, 0.22f, aim);
        configuration.animator.SetInteger("Arms Phase", 11);

        yield return new WaitForSeconds(throwDelay);
        DoBonusAttack(aim, bullet, bulletRig);
    }

    public override void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        bullet.transform.position = configuration.weapon.transform.position;
        bullet.transform.localEulerAngles = configuration.weapon.transform.localEulerAngles;
        bullet.transform.localScale = configuration.weapon.transform.localScale;
        configuration.weapon.gameObject.SetActive(false);

        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, configuration.weapon.transform, side);
        bulletRig.velocity = aim * configuration.bulletSpeed;
        bulletRig.angularVelocity = Random.Range(scytheSpinSpeed.x, scytheSpinSpeed.y) * Mathf.Sign(-aim.x);
    }
}

