using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    public Camera camera;
    public override Vector2 getAim() => (Input.mousePosition - camera.WorldToScreenPoint(lookAround.shootingArm.position)).normalized;

    public override void Update()
    {
        configuration = weaponSystem.weaponConfiguration;
        attackProgress = weaponSystem.IWeapon.attackProgress;

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1))
            RightClickAttack();

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.GetAmmo <= 0 || attackProgress != "finished")
            return;

        if (configuration.weaponType == Type.singleFire && Input.GetMouseButtonDown(0))
        {
            if (combatMode == "gun" || combatMode == "handheld")
                Attack();

            else if (combatMode == "meelee")
                NonAmmoAttack();
        }

        else if (configuration.weaponType == Type.spamFire && Input.GetMouseButton(0) && countdownBtwnShots <= 0f)
        {
            countdownBtwnShots = 1f / configuration.fireRateInfo;
            Attack();
        }

        else if (configuration.weaponType == Type.chargeUpFire && Input.GetMouseButton(0))
            Attack();
    }

    // For "hold-fire" weapons, firing needs to be called every frame.
    // Called by the Late Update method of the player's LookAround script, 
    // after the weapon rotation is updated over there 
    public override void LateUpdateAfterWeaponRotation()
    {
        if (weaponSystem.GetAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire && Input.GetMouseButton(0))
            Attack();
    }
}
