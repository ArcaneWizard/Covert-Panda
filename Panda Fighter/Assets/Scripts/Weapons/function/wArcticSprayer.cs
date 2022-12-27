using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wArcticSprayer : WeaponMechanics
{
    private Transform gizmo = null;

    public override IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        DoAttack(aim, bullet, rig);
        yield return null;
    }

    public override void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        reusableWeaponMethods.configureReusedBullet(bullet, rig, config.bulletSpawnPoint, side);

        gizmo = bullet;

        reusableWeaponMethods.shootBulletInArc(aim, bullet, rig, new Vector2(1f, 1.1f), config.bulletSpeed, false);
    }

     void OnDrawGizmos()
    {
        if (gizmo == null)
            return;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gizmo.position, 0.5f);

        gizmo = null;
    }
}
