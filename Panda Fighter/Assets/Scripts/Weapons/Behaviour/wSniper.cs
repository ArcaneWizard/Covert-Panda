using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : WeaponBehaviour
{
    protected override void startAttack() 
    {
        PhysicalBullet bullet = WeaponBehaviourHelper.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);
        bullet.transform.GetComponent<SniperBeam>().ShowBeam();
        bullet.transform.GetComponent<Bullet>().StartCollisionDetection(aim, BulletMovementAfterFiring.StraightLine, false);
    }
}
