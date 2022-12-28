using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    public GameObject grenadeHeld { get; private set; }

    protected CentralLookAround lookAround;
    protected CentralWeaponSystem weaponSystem;
    protected CentralGrenadeSystem grenadeSystem;
    protected Health health;

    private Transform bullet;
    private Rigidbody2D bulletRig;

    protected virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        grenadeSystem = transform.GetComponent<CentralGrenadeSystem>();
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

            grenadeHeld.transform.position = grenadeSystem.CurrentGrenadeConfiguration.bulletSpawnPoint.position;
            grenadeHeld.transform.rotation = grenadeSystem.CurrentGrenadeConfiguration.bulletSpawnPoint.rotation;
            grenadeHeld.SetActive(true);
        }
    }

    public abstract Vector2 GetAim();

    public void ReleasedGrenade() => grenadeHeld = null;

    protected void DeployGrenade()
    {
        Transform grenade = grenadeSystem.UseOneGrenade().transform;
        Rigidbody2D grenadeRig = grenade.GetComponent<Rigidbody2D>();
        grenadeHeld = grenade.gameObject;
        grenadeSystem.CurrentGrenadeImplementation.DoSetupAttack(GetAim(), grenade, grenadeRig);
    }

    protected void Attack()
    {
        bullet = weaponSystem.UseOneBullet().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();
        weaponSystem.CurrentWeaponImplementation.DoSetupAttack(GetAim(), bullet, bulletRig);
    }

    protected void NonAmmoAttack()
    {
        weaponSystem.CurrentWeaponImplementation.DoSetupAttack(GetAim(), null, null);
    }

   /* protected void RightClickAttack()
    {
        bullet = weaponSystem.getLastBullet().transform;
        bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

        weaponSystem.CurrentWeapon.DoBonusSetupAttack(GetAim(), bullet, bulletRig);
    }*/

}
