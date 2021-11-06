using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wBoomerang : IWeapon 
{    
    protected float boomerangSpeed = 52;
    protected Vector2 boomerangSpinSpeed = new Vector2(600, 1050);
    
    public override void Awake() {base.Awake(); config.IK_Coordinates = AimingDir.boomerangAiming;}

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig) 
    {
        reusableWeaponMethods.configureReusedBullet(bullet, bulletRig, config.bulletSpawnPoint);
        reusableWeaponMethods.shootBulletInStraightLine(aim, bullet, bulletRig, boomerangSpeed);

        bullet.transform.GetComponent<Animator>().SetBool("glare", false);
        bulletRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
    }

    public override IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
    {
        DoBonusAttack(aim, bullet, bulletRig);
        yield return null;
    }

    public override void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D bulletRig)
     {
        bulletRig.velocity = Quaternion.Euler(0, 0, 90 * -Mathf.Sign(aim.x)) * aim * boomerangSpeed;
        bulletRig.angularVelocity = Random.Range(boomerangSpinSpeed.x, boomerangSpinSpeed.y) * (Random.Range(0, 2) * 2 - 1);
        bullet.transform.GetComponent<Animator>().SetBool("glare", true);  
    }
}
