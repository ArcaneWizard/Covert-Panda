using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class wLavaPistol : WeaponBehaviour
{
    protected override void startAttack()
    {
        WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, 
            weaponConfiguration, side, extraSettings);

        void extraSettings(Transform bullet)
        {
            bullet.localEulerAngles = Vector3.zero;
            bullet.transform.GetComponent<Bullet>().StartCollisionDetection(aim, BulletMovementAfterFiring.StraightLine, false);
        }
    }
}
