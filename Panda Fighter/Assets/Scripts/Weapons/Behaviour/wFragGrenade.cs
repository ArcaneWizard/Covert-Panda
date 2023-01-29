using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class wFragGrenade : WeaponBehaviour
{
    protected float grenadeThrowForce = 2200;
    protected float grenadeYForce = -20;

    public override void ConfigureUponSwitchingToWeapon()
    {
        base.ConfigureUponSwitchingToWeapon();
        weaponConfiguration.Animator.SetInteger("Arms Phase", 0);
    }

    protected override IEnumerator attack(Vector2 aim)
    {
        StartCoroutine(base.attack(aim));
        weaponConfiguration.Arms[1].transform.parent.GetComponent<Animator>().Play(0, -1);
        weaponConfiguration.Arms[1].SetActive(true);

        float wait = WeaponAction.CalculateTimeB4ReleasingGrenade(0.02f, 0.2f, aim);
        yield return new WaitForSeconds(wait);

        Transform grenade = WeaponAction.SpawnGrenade(aim, grenadeSystem, weaponConfiguration, side, false);
        grenade.transform.right = -aim;

        grenade.GetComponent<Collider2D>().isTrigger = false;
        grenade.GetComponent<FragGrenade>().startExplosionTimer();

        Rigidbody2D rig = grenade.GetComponent<Rigidbody2D>();
        Vector2 unadjustedForce = weaponConfiguration.BulletSpeed * 40 * aim * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
        rig.AddForce(unadjustedForce * rig.mass);

        yield return new WaitForSeconds(0.6f - wait);
        weaponConfiguration.Arms[1].SetActive(false);
        confirmAttackFinished();
    } 
}
