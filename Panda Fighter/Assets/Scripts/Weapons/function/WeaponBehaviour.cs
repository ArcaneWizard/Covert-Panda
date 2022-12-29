using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This abstract class is utilized to implement a weapon's behaviour upon being
// equipped, and how it attacks (ex. how the gun fires bullets). It tracks the
// progress of the attack (Started or Finished). Additionally, it allows
// weapons to implement a 2nd/bonus attack.

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

    // behaviour when weapon is equipped
    public virtual void UponEquippingWeapon() { return; }

    // behaviour when creature does a default attack with this weapon
    public virtual IEnumerator Attack(Vector2 aim)
    {
        attackProgress = Progress.Started;
        yield return null;
    }

    // behaviour when creature does a bonus attack (if possible) with
    // this weapon
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


