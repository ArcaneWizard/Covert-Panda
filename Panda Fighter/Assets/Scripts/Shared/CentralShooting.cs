using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    [HideInInspector]
    public GameObject weaponHeld = null;
    public string combatMode = "gun";

    protected CentralWeaponSystem weaponSystem;
    protected CentralLookAround lookAround;

    private List<GameObject> WeaponSetup = new List<GameObject>();
    private Transform bulletSpawnPoint, bullet;
    private Rigidbody2D bulletRig;

    public virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
    }

    private void LateUpdate()
    {
        if (combatMode == "handheld")
        {
            if (weaponSystem.getAmmo > 0)
            {
                weaponHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                weaponHeld.transform.GetComponent<Collider2D>().isTrigger = true;

                weaponHeld.transform.position = bulletSpawnPoint.position;
                weaponHeld.transform.rotation = bulletSpawnPoint.rotation;
                weaponHeld.SetActive(true);
            }
        }
    }

    public void updateWeaponHeldForHandheldWeapons() => weaponHeld = weaponSystem.getBullet;

    //specify which limbs, weapon and aim target to activate (the latter helps a weapon track while aiming) 
    public void configureWeaponAndArms()
    {
        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(false);
        WeaponSetup.Clear();

        IWeapon Iweapon = weaponSystem.weapon;

        if (Iweapon.config.aimTarget != null)
            lookAround.setAimTarget(Iweapon.config.aimTarget);
        bulletSpawnPoint = Iweapon.config.bulletSpawnPoint;

        if (Iweapon.config.weapon != null)
            WeaponSetup.Add(Iweapon.config.weapon);

        foreach (GameObject limb in Iweapon.config.limbs)
            WeaponSetup.Add(limb);

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);

        List<Vector2> aiming = Iweapon.config.IK_Coordinates;
        lookAround.calculateShoulderAngles(aiming);
    }

    protected void Attack()
    {
        bullet = weaponSystem.getBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.useOneAmmo();
        weaponSystem.weapon.DoSetupAttack(getAim(), bullet, bulletRig);
    }

    protected void NonAmmoAttack()
    {
        bullet = weaponSystem.getBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.weapon.DoSetupAttack(getAim(), bullet, bulletRig);
    }

    protected void RightClickAttack()
    {
        bullet = weaponSystem.getLastBullet().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        weaponSystem.weapon.DoBonusSetupAttack(getAim(), bullet, bulletRig);
    }

    public abstract Vector2 getAim();

}
