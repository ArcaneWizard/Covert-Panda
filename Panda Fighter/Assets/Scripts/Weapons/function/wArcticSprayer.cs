using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wArcticSprayer : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        Vector2 forceMultiplier = new Vector2(1.0f, 1.1f);
        Vector2 forceOffset = Vector2.zero;

        WeaponAction.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
            weaponSystem, weaponConfiguration, side, false);

        ConfirmAttackFinished();
        yield return null;
    }
}
