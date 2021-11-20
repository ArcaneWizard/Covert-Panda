using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    public Camera camera;

    private float countdownBtwnShots = 0f;
    private float timeHeld = 0f;

    private WeaponConfig config;
    private String attackProgress;

    public override Vector2 getAim() => (Input.mousePosition - camera.WorldToScreenPoint(lookAround.shootingArm.position)).normalized;

    void Update()
    {
        config = weaponSystem.weaponConfig;
        attackProgress = weaponSystem.weapon.attackProgress;

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1))
            RightClickAttack();

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.weaponSelected == null || weaponSystem.getAmmo <= 0 || attackProgress != "finished")
            return;

        if (config.weaponType == Type.singleFire && Input.GetMouseButtonDown(0))
        {
            if (combatMode == "gun" || combatMode == "handheld")
                Attack();

            else if (combatMode == "meelee")
                NonAmmoAttack();
        }

        else if (config.weaponType == Type.spamFire && Input.GetMouseButton(0) && countdownBtwnShots <= 0f)
        {
            countdownBtwnShots = 1f / weaponSystem.weaponConfig.fireRateInfo;
            Attack();
        }

        else if (config.weaponType == Type.holdFire && Input.GetMouseButtonDown(0))
            Attack();
    }
}
