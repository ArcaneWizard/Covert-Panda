using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    private Progress attackProgress;
    private float countdownBtwnShots = 0f;

    public override Vector2 GetAim() => lookAround.directionToLook;

    private void Update()
    {
        if (health.isDead)
        {
            countdownBtwnShots = 0f;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab) && grenadeSystem.GrenadesLeft > 0)
            DeployGrenade();

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        WeaponConfiguration configuration = weaponSystem.CurrentWeaponConfiguration;
        WeaponImplementation implementation = weaponSystem.CurrentWeaponImplementation;

        if (weaponSystem.CurrentAmmo <= 0 || implementation.attackProgress != Progress.Finished || countdownBtwnShots > 0f)
            return;

        if (configuration.weaponType != WeaponType.holdFire && Input.GetMouseButtonDown(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            AttackWithWeapon();
        }

        else if (configuration.weaponType == WeaponType.holdFire && Input.GetMouseButton(0))
            AttackWithWeapon();
    }
}
