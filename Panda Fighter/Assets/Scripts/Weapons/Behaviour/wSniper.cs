using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : WeaponBehaviour
{
    protected override void attack(Vector2 aim) 
    {
        PhysicalBullet bullet = CommonWeaponBehaviours.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);
        bullet.transform.GetComponent<SniperBeam>().ShowBeam();
        bullet.transform.GetComponent<Bullet>().OnFire(aim);
    }
}
