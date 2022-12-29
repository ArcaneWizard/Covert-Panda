using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wArcticSprayer : WeaponImplementation
{
    private Transform gizmo = null;

    public override IEnumerator StartAttack(Vector2 aim)
    {
        StartCoroutine(base.StartAttack(aim));
        gizmo = WeaponAction.ShootBulletInArc(aim, weaponSystem, weaponConfiguration, side, false);
        FinishAttack();
        yield return null;
    }

     void OnDrawGizmos()
    {
        if (gizmo == null)
            return;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(gizmo.position, 0.5f);

        gizmo = null;
    }
}
