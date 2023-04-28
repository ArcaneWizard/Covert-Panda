using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wPlasmaOrb : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Vector2 forceMultiplier = new Vector2(1.4f, 1.5f);
        Vector2 forceOffset = new Vector2(0f, 0f);

       WeaponBehaviourHelper.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
            weaponSystem, weaponConfiguration, side);

        confirmAttackFinished();
        yield return null;
    }
}