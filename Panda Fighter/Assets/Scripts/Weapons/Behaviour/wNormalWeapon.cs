using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wNormalWeapon : WeaponBehaviour
{
    protected override void startAttack()
    {
        WeaponBehaviourHelper.SpawnAndShootBulletForward(aim, weaponSystem, weaponConfiguration, side);
    }
}
