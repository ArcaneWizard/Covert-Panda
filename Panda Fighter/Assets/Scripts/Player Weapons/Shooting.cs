using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{


    void Update()
    {
        //Weapons where you left click each time to shoot
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                base.retrieveWeaponAmmunitionThenAttack();

            else if (combatMode == "handheld" && !weaponAttacks.attackAnimationPlaying && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
                base.retrieveWeaponAmmunitionThenAttack();

            else if (combatMode == "meelee" && !weaponAttacks.attackAnimationPlaying)
            {
                weaponAttacks.updateEntities(ammunition, objectRig);
                weaponAttacks.meeleeAttack(weaponSystem.weaponSelected);
            }

        }

        //Weapons where you can hold the left mouse button down to continously use and drain the weapon
        if (Input.GetMouseButton(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && timeLeftBtwnShots <= 0 && weaponSystem.getWeapon().tag == "spamFire")
            {
                ammunition = weaponSystem.getWeapon();
                weaponSystem.useOneAmmo();
                objectRig = ammunition.transform.GetComponent<Rigidbody2D>();

                weaponAttacks.updateEntities(ammunition, objectRig);
                weaponAttacks.spamFireAttack(weaponSystem.weaponSelected);
            }
        }

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1))
        {
            weaponAttacks.curveBoomerang();
            StartCoroutine(weaponAttacks.throwScythe());
        }

        if (timeLeftBtwnShots > 0)
            timeLeftBtwnShots -= Time.deltaTime;
    }
}
