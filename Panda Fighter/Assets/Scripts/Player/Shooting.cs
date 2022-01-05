using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    public Camera camera;

    private float countdownBtwnShots = 0f;

    private WeaponConfiguration configuration;
    private String attackProgress;

    public override Vector2 getAim() => (Input.mousePosition - camera.WorldToScreenPoint(lookAround.shootingArm.position)).normalized;

    private void Update()
    {
        configuration = weaponSystem.weaponConfiguration;
        attackProgress = weaponSystem.IWeapon.attackProgress;

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1))
            RightClickAttack();

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.GetAmmo <= 0 || attackProgress != "finished" || countdownBtwnShots > 0f || health.isDead)
            return;

        if (configuration.weaponType == Type.singleFire && Input.GetMouseButtonDown(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            if (combatMode == "gun" || combatMode == "handheld")
                Attack();

            else if (combatMode == "meelee")
                NonAmmoAttack();
        }

        else if (configuration.weaponType == Type.spamFire && Input.GetMouseButton(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            Attack();
        }

        /*else if (configuration.weaponType == Type.chargeUpFire && Input.GetMouseButton(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            Attack();
        }*/
    }

    public void LateLateUpdate()
    {
        if (weaponSystem.GetAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire && Input.GetMouseButton(0))
            Attack();
    }
}
