using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    //public GameObject grenadeHeld { get; private set; }

    protected CentralLookAround lookAround;
    protected CentralWeaponSystem weaponSystem;
    protected CentralGrenadeSystem grenadeSystem;
    protected CentralPhaseTracker phaseTracker;
    protected Health health;

    private Transform bullet;
    private Rigidbody2D bulletRig;

    private Coroutine attack;

    protected virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        grenadeSystem = transform.GetComponent<CentralGrenadeSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
        phaseTracker = transform.GetComponent<CentralPhaseTracker>();
        health = transform.GetComponent<Health>();
        
        //grenadeHeld = null;
    }

   // protected virtual void FixedUpdate()
    //{
      //  weaponSystem.CurrentWeaponConfiguration.MainArmIKTracker.gameObject.SetActive(!phaseTracker.IsDoingSomersault);
    //}

    /*private void LateUpdate()
    {
        if (health.isDead)
            return;

        if (grenadeHeld != null)
        {
            grenadeHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            grenadeHeld.transform.GetComponent<Collider2D>().isTrigger = true;

            grenadeHeld.transform.position = grenadeSystem.CurrentGrenadeConfiguration.BulletSpawnPoint.position;
            grenadeHeld.transform.rotation = grenadeSystem.CurrentGrenadeConfiguration.BulletSpawnPoint.rotation;
            grenadeHeld.SetActive(true);
        }
    }*/

    public abstract Vector2 GetAim();

    //public void ReleasedGrenade() => grenadeHeld = null;
    //protected void DeployGrenade() => grenadeSystem.CurrentGrenadeImplementation.attack(GetAim()));

    // Attack. Use current weapon aim if not mid-somersault. Else, use rotation of gun 
    protected void AttackWithWeapon()
    {
        weaponSystem.CurrentWeaponBehaviour.Attack(GetAim());
    }

    public virtual void Reset() { }

    /* protected void RightClickAttack()
     {
         bullet = weaponSystem.getLastBullet().transform;
         bulletRig = bullet.transform.GetComponent<Rigidbody2D>();

         weaponSystem.CurrentWeapon.DoBonusSetupAttack(GetAim(), bullet, bulletRig);
     }*/

}
