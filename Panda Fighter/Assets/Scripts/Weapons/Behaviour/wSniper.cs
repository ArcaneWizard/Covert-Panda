using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        Transform bullet = WeaponAction.SpawnBullet(aim, weaponSystem, weaponConfiguration, side, false);
        bullet.GetComponent<SniperBeam>().ShowBeam();

        confirmAttackFinished();
        yield return null;
    }
}
