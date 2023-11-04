using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using System;

public class wArcticSprayer : WeaponBehaviour
{
    private static Vector2 forceMultiplier = new Vector2(1.0f, 1.1f);
    private static Vector2 forceOffset = Vector2.zero;

    protected override void attack(Vector2 aim)
    {
        Transform bullet = CommonWeaponBehaviours.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
            weaponSystem, weaponConfiguration, side);
    }
}
