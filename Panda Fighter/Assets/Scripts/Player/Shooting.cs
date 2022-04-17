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

    public override void Awake() 
    {
        base.Awake();
        camera = transform.parent.parent.parent.GetComponent<References>().Camera;
    }

    public override Vector2 GetAim() 
    {
        Vector3 aim = Input.mousePosition - camera.WorldToScreenPoint(lookAround.shootingArm.position);
        return new Vector2(aim.x, aim.y).normalized;
    }

    private void Update()
    {
        if (health.isDead)
            return;

        configuration = weaponSystem.CurrentWeaponConfiguration;
        attackProgress = weaponSystem.CurrentWeapon.attackProgress;

        if (Input.GetKeyDown(KeyCode.Tab) && weaponSystem.GetAmmo(WeaponTags.Grenades) > 0)
            DeployGrenade();

        ShootGunOrUseMeeleeWeapon();
    }

    private void ShootGunOrUseMeeleeWeapon() 
    {
        configuration = weaponSystem.CurrentWeaponConfiguration;
        attackProgress = weaponSystem.CurrentWeapon.attackProgress;

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.CurrentAmmo <= 0 || attackProgress != "finished" || countdownBtwnShots > 0f)
            return;

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
    }

    public void LateLateUpdate()
    {
        if (health.isDead || weaponSystem.CurrentAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire && Input.GetMouseButton(0))
            Attack();
    }
}
