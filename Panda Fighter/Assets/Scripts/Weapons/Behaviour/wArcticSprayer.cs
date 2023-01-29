using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wArcticSprayer : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Vector2 forceMultiplier = new Vector2(1.0f, 1.1f);
        Vector2 forceOffset = Vector2.zero;

        WeaponAction.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
            weaponSystem, weaponConfiguration, side, false);

        confirmAttackFinished();
        yield return null;
    }
}
