using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class Shooting : CentralShooting
{
    private float timerBtwnShots = 0f;
    private float chargeUpTimer = 0f;
    private bool canExecuteChargedAttack = true;

    public override Vector2 GetAim() => lookAround.directionToLook;

    public override void Reset()
    {
        timerBtwnShots = 0f;
        chargeUpTimer = 0f;
        canExecuteChargedAttack = true;
    }

    private void Update()
    {
        if (health.IsDead)
            return;

        //if (Input.GetKeyDown(KeyCode.Tab) && grenadeSystem.GrenadesLeft > 0)
        //  DeployGrenade();

        WeaponConfiguration configuration = weaponSystem.CurrentWeaponConfiguration;

        if (configuration.FiringMode != FiringMode.ChargeUpFire)
            canExecuteChargedAttack = true;

        if (weaponSystem.CurrentAmmo <= 0 || weaponSystem.CurrentWeaponBehaviour.attackProgress != AttackProgress.Finished)
            return;

        if (configuration.FiringMode == FiringMode.SingleFire && Input.GetMouseButtonDown(0) && timerBtwnShots <= 0f)
        {
            timerBtwnShots = 1 / configuration.FireRateInfo;
            AttackWithWeapon();
        }

        else if (configuration.FiringMode == FiringMode.SpamFire && Input.GetMouseButton(0) && timerBtwnShots <= 0f)
        {
            timerBtwnShots = 1 / configuration.FireRateInfo;
            AttackWithWeapon();
        }

        else if (configuration.FiringMode == FiringMode.ChargeUpFire)
        {
            // set the charge up timer when the right mouse button is first pressed
            if (Input.GetMouseButtonDown(0))
            {
                chargeUpTimer = configuration.FireRateInfo;
                weaponSystem.CurrentWeaponBehaviour.StartChargingUp();
            }

            // tick down the timer whlie the right mouse button is held
            if (Input.GetMouseButton(0) && chargeUpTimer > 0f)
                chargeUpTimer -= Time.deltaTime;

            // once the the timer is up, shoot with the weapon if it hasn't already shot
            if (Input.GetMouseButton(0) && chargeUpTimer <= 0f && canExecuteChargedAttack)
            {
                AttackWithWeapon();
                canExecuteChargedAttack = false;
            }

            // once the mouse button is let go, the weapon is allowed to charge up again 
            if (!canExecuteChargedAttack && Input.GetMouseButtonUp(0))
                canExecuteChargedAttack = true;

            if (Input.GetMouseButtonUp(0))
                weaponSystem.CurrentWeaponBehaviour.StopChargingUp();
        }

        else if (configuration.FiringMode == FiringMode.ContinousBeam && Input.GetMouseButton(0))
            AttackWithWeapon();
    }

    private void FixedUpdate()
    {
        if (timerBtwnShots > 0f)
            timerBtwnShots -= Time.fixedDeltaTime;
    }
}
