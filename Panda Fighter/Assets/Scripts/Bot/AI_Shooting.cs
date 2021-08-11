using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{

    void Update()
    {
        //Weapons where you left click each time to shoot
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                weaponAttacks.weaponAttack();

            else if (combatMode == "handheld" && !weaponAttacks.attackAnimationPlaying && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                weaponAttacks.weaponAttack();

            else if (combatMode == "meelee" && !weaponAttacks.attackAnimationPlaying)
                weaponAttacks.meeleeAttack();
        }

        //Weapons where you can hold the left mouse button down to continously use and drain the weapon
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
                weaponAttacks.weaponAttack();
        }

        if (timeLeftBtwnShots > 0)
            timeLeftBtwnShots -= Time.deltaTime;
    }
}
