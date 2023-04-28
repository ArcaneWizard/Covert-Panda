using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wSniper : WeaponBehaviour
{
    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));

        PhysicalBullet bullet = WeaponBehaviourHelper.SpawnBullet(aim, weaponSystem, weaponConfiguration, side);
        bullet.transform.GetComponent<SniperBeam>().ShowBeam();

        confirmAttackFinished();
        yield return null;
    }
}
