using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    public GameObject weaponHeld { get; private set; }
    public string combatMode { get; private set; }

    protected CentralWeaponSystem weaponSystem;
    protected CentralLookAround lookAround;
    protected Health health;

    private List<GameObject> WeaponSetup = new List<GameObject>();
    private Transform bullet;
    private Rigidbody2D bulletRig;

    public virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
        weaponHeld = null;
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        if (combatMode == "handheld")
        {
            if (weaponSystem.GetAmmo > 0)
            {
                weaponHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                weaponHeld.transform.GetComponent<Collider2D>().isTrigger = true;

                weaponHeld.transform.position = weaponSystem.weaponConfiguration.bulletSpawnPoint.position;
                weaponHeld.transform.rotation = weaponSystem.weaponConfiguration.bulletSpawnPoint.rotation;
                weaponHeld.SetActive(true);
            }
        }
    }

    public void updateWeaponHeldForHandheldWeapons() => weaponHeld = weaponSystem.GetBullet;
    public void updateCombatMode(string combatMode) => this.combatMode = combatMode;

    //specify which limbs, weapon and aim target to activate (the latter helps a weapon track while aiming) 
    public void configureWeaponAndArms()
    {
        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject things in WeaponSetup)
            things.SetActive(false);
        WeaponSetup.Clear();

        WeaponConfiguration config = weaponSystem.weaponConfiguration;
        if (config.aimTarget != null)
            lookAround.setAimTarget(config.aimTarget);

        if (config.weapon != null)
            WeaponSetup.Add(config.weapon);
        foreach (GameObject limb in config.limbs)
            WeaponSetup.Add(limb);
        foreach (GameObject things in WeaponSetup)
            things.SetActive(true);

        lookAround.calculateShoulderAngles(config.IK_Coordinates);
    }

    protected void Attack()
    {
        bullet = weaponSystem.GetBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.useOneAmmo();
        weaponSystem.IWeapon.DoSetupAttack(getAim(), bullet, bulletRig);
    }

    protected void NonAmmoAttack()
    {
        bullet = weaponSystem.GetBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.IWeapon.DoSetupAttack(getAim(), bullet, bulletRig);
    }

    protected void RightClickAttack()
    {
        bullet = weaponSystem.getLastBullet().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        weaponSystem.IWeapon.DoBonusSetupAttack(getAim(), bullet, bulletRig);
    }

    public abstract Vector2 getAim();

}
