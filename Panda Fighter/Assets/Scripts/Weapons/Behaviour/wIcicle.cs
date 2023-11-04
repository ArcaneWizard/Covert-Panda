using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wIcicle : WeaponBehaviour
{
    protected override void attack(Vector2 aim)
    {
        CommonWeaponBehaviours.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
    }
}
