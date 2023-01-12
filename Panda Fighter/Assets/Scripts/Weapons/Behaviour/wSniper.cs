using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : WeaponBehaviour
{
    public override IEnumerator Attack(Vector2 aim)
    {
        StartCoroutine(base.Attack(aim));

        Transform bullet = WeaponAction.SpawnBullet(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.GetComponent<SniperBeam>().ShowBeam();

        ConfirmAttackFinished();
        yield return null;
    }
}
