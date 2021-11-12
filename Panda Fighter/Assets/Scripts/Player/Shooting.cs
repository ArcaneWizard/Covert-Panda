using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    public Camera camera;
    private float countdownBtwnShots = 0f;

    public override Vector2 getAim() => (Input.mousePosition - camera.WorldToScreenPoint(lookAround.shootingArm.position)).normalized;

    void Update()
    {
        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (weaponSystem.weaponSelected == null)
            return;

        //Weapons where you left click each time to shoot
        if (Input.GetMouseButtonDown(0) && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
        {
            if (combatMode == "gun")
                Attack();

            else if (combatMode == "handheld" && weaponSystem.getWeaponConfig().attackProgress == "finished")
                Attack();

            else if (combatMode == "meelee" && weaponSystem.getWeaponConfig().attackProgress == "finished")
                MeeleeAttack();
        }

        //Weapons where you hold left click to shoot
        if (Input.GetMouseButton(0) && countdownBtwnShots <= 0f && weaponSystem.getAmmo() > 0)
        {
            if (combatMode == "gun" && weaponSystem.getWeapon().tag == "spamFire")
            {
                countdownBtwnShots = 1f / weaponSystem.getWeaponConfig().config.ratePerSecond;
                Attack();
            }
        }

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1))
            RightClickAttack();
    }
}
