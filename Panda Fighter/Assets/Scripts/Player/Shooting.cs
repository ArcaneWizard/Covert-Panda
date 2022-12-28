using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    private Camera camera;
    private WeaponConfiguration configuration;

    private float countdownBtwnShots = 0f;
    private String attackProgress;

    public override Vector2 GetAim() => lookAround.directionToLook;

    protected override void Awake() 
    {
        base.Awake();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;
    }

    private void Update()
    {
        if (health.isDead)
            return;

        if (Input.GetKeyDown(KeyCode.Tab) && grenadeSystem.GetGrenadeCount() > 0)
            DeployGrenade();

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.CurrentWeaponAmmo <= 0 || attackProgress != "finished" || countdownBtwnShots > 0f)
            return;

        configuration = weaponSystem.CurrentWeaponConfiguration;
        attackProgress = weaponSystem.CurrentWeapon.attackProgress;
        
        if (configuration.weaponType == Type.singleFire && Input.GetMouseButtonDown(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            if (combatMode == "gun")
                Attack();

            else if (combatMode == "meelee")
                NonAmmoAttack();
        }

        else if (configuration.weaponType == Type.spamFire && Input.GetMouseButton(0))
        {
            countdownBtwnShots = configuration.fireRateInfo;
            Attack();
        }

        else if (configuration.weaponType == Type.holdFire && Input.GetMouseButton(0))
            Attack();
    }
}
