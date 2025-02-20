using UnityEngine;

public abstract class CentralShooting : MonoBehaviour
{
    //public GameObject grenadeHeld { get; private set; }

    protected CentralLookAround lookAround;
    protected CentralWeaponSystem weaponSystem;
    protected CentralGrenadeSystem grenadeSystem;
    protected Health health;

    public abstract void ConfigureUponPullingOutWeapon();

    protected virtual void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        grenadeSystem = transform.GetComponent<CentralGrenadeSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
        health = transform.GetComponent<Health>();
    }

    protected abstract Vector2 GetAim();

    // Attack. Use current weapon aim if not mid-somersault. Else, use rotation of gun 
    protected void AttackWithWeapon()
    {
        weaponSystem.CurrentWeaponBehaviour.Attack(GetAim());
    }
}
