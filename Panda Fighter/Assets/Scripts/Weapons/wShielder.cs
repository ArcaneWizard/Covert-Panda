using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wShielder : IWeapon {
    
    private float shielderBulletSpeed = 45;
 
    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    { 
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig) {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint);
        bullet.transform.right = aim;
        rig.velocity = aim * shielderBulletSpeed;
    }
}
