using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Shooting : CentralShooting
{
    private Transform bullet;
    private Rigidbody2D bulletRig;

    public Camera camera;
    public Transform shootingArm;

    void Update()
    {
        //Weapons where you left click each time to shoot
        if (Input.GetMouseButtonDown(0) && weaponSystem.weaponSelected != null)
        {
            if (combatMode == "gun" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            Attack();

            else if (combatMode == "handheld" && weaponSystem.getWeaponConfig().attackProgress == "finished" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            ThrowAttack();

            else if (combatMode == "meelee" && weaponSystem.getWeaponConfig().attackProgress == "finished" && weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
            MeeleeAttack();
        }

        //Weapons where you right click for a diff attack or weapon mechanic
        if (Input.GetMouseButtonDown(1) && weaponSystem.weaponSelected != null)
            RightClickAttack();
    }


    private void Attack() {
        weaponSystem.useOneAmmo();
        bullet = weaponSystem.getWeapon().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        
        weaponSystem.getWeaponConfig().DoSetupAttack(getAim(), bullet, bulletRig);
    }

    private void ThrowAttack() {
        weaponSystem.useOneAmmo();
        bullet = weaponSystem.getWeapon().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        newWeaponHeld = bullet.gameObject;
        
        weaponSystem.getWeaponConfig().DoSetupAttack(getAim(), bullet, bulletRig);
    }

    private void MeeleeAttack() {
        bullet = weaponSystem.getWeapon().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        
        weaponSystem.getWeaponConfig().DoSetupAttack(getAim(), bullet, bulletRig);
    }

    private void RightClickAttack() {
        bullet = weaponSystem.getWeapon().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        weaponSystem.getWeaponConfig().DoBonusSetupAttack(getAim(), bullet, bulletRig);
    }

    private Vector2 getAim()
    {
        return (Input.mousePosition - camera.WorldToScreenPoint(shootingArm.position)).normalized;
    }
}
