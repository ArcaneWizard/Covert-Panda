using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
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
        WeaponBehaviour behaviour = weaponSystem.CurrentWeaponBehaviour;

        if (weaponSystem.CurrentAmmo <= 0 || behaviour.attackProgress != AttackProgress.Finished || countdownBtwnShots > 0f)
            return;

        if (configuration.WeaponType == FiringModes.singleFire && Input.GetMouseButtonDown(0))
        {
            countdownBtwnShots = configuration.FireRateInfo;
            AttackWithWeapon();
        }

        else if (configuration.WeaponType == FiringModes.spamFire && Input.GetMouseButton(0))
        {
            countdownBtwnShots = configuration.FireRateInfo;
            AttackWithWeapon();
        }

        else if (configuration.WeaponType == FiringModes.holdFire && Input.GetMouseButton(0))
            AttackWithWeapon();
    }
}
