using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : WeaponBehaviour
{
    private static Vector2 forceMultiplier = new Vector2(1.4f, 1.5f);
    private static Vector2 forceOffset = new Vector2(0f, 0f);

    protected override void startAttack()
    {
       WeaponBehaviourHelper.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
            weaponSystem, weaponConfiguration, side);
    }
}