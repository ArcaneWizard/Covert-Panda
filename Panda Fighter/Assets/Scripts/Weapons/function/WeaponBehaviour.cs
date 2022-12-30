using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This abstract class is utilized to implement a weapon's behaviour:
// aka how it fires/attacks, and any animation/behaviour the moment its equipped.
// The progress of an "attack" is tracked, useful to know for weapons
// that charge up or meelee weapons that swing. Additionally, weapons
// can implement a 2nd/bonus attack.

public abstract class WeaponBehaviour : MonoBehaviour
{
    public Progress attackProgress { get; protected set; }
    public Progress bonusAttackProgress { get; protected set; }

    protected CentralShooting shooting;
    protected CentralPhaseTracker phaseTracker;
    protected WeaponConfiguration weaponConfiguration;
    protected CentralWeaponSystem weaponSystem;
    protected CentralGrenadeSystem grenadeSystem;
    protected Side side;

    public void Initialize(WeaponConfiguration weaponConfiguration, CentralGrenadeSystem grenadeSystem,
        CentralWeaponSystem weaponSystem)
    {
        this.weaponConfiguration = weaponConfiguration;
        this.grenadeSystem = grenadeSystem;
        this.weaponSystem = weaponSystem;
    }

    // what happens right when the weapon is equipped
    public virtual void UponEquippingWeapon() { return; }

    // default attack with this weapon. The aim direction is specified
    public virtual IEnumerator Attack(Vector2 aim)
    {
        attackProgress = Progress.Started;
        yield return null;
    }

    // bonus attack (if possible) with this weapon. The aim direction is specified
    public virtual IEnumerator BonusAttack(Vector2 aim)
    {
        bonusAttackProgress = Progress.Started;
        yield return null;
    }

    // Refactor later: referenced when you switch to a different weapons (make private in the future)
    public void ResetAttackProgress()
    {
        attackProgress = Progress.Finished;
        bonusAttackProgress = Progress.Finished;
    }

    protected void ConfirmAttackFinished() => attackProgress = Progress.Finished;
    protected void ConfirmBonusAttackFinished() => bonusAttackProgress = Progress.Finished;

    void Awake()
    {
        phaseTracker = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralPhaseTracker>();
        side = transform.parent.parent.parent.GetComponent<Role>().side;

        ResetAttackProgress();
    }
}


