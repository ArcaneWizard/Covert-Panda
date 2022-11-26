using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    public GameObject grenadeHeld { get; private set; }
    public string combatMode { get; private set; }

    protected CentralLookAround lookAround;
    protected CentralWeaponSystem weaponSystem;
    protected Health health;

    private List<GameObject> WeaponSetup = new List<GameObject>();
    private Transform bullet;
    private Rigidbody2D bulletRig;

    protected virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
        
        grenadeHeld = null;
    }

    private void LateUpdate()
    {
        if (health.isDead)
            return;

        if (grenadeHeld != null)
        {
            grenadeHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            grenadeHeld.transform.GetComponent<Collider2D>().isTrigger = true;

            grenadeHeld.transform.position = weaponSystem.GetWeaponConfiguration(WeaponTags.Grenades).bulletSpawnPoint.position;
            grenadeHeld.transform.rotation = weaponSystem.GetWeaponConfiguration(WeaponTags.Grenades).bulletSpawnPoint.rotation;
            grenadeHeld.SetActive(true);
        }
    }

    public abstract Vector2 GetAim();
    public void UpdateCombatMode(string combatMode) => this.combatMode = combatMode;
    public void LetGoOffAnyGrenades() => grenadeHeld = null;

    protected void Attack()
    {
        bullet = weaponSystem.CurrentBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.useOneAmmo();
        weaponSystem.CurrentWeapon.DoSetupAttack(GetAim(), bullet, bulletRig);
    }

    protected void NonAmmoAttack()
    {
        bullet = weaponSystem.CurrentBullet.transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.CurrentWeapon.DoSetupAttack(GetAim(), bullet, bulletRig);
    }
    
    //TO-DO: should actually useup "Grenade ammo"
    protected void DeployGrenade()
    {
        bullet = weaponSystem.GetBulletAndUseAmmo(WeaponTags.Grenades).transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        grenadeHeld = bullet.gameObject;

        weaponSystem.GetWeapon(WeaponTags.Grenades).DoSetupAttack(GetAim(), bullet, bulletRig);
    }

    protected void RightClickAttack()
    {
        bullet = weaponSystem.getLastBullet().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        weaponSystem.CurrentWeapon.DoBonusSetupAttack(GetAim(), bullet, bulletRig);
    }

}
