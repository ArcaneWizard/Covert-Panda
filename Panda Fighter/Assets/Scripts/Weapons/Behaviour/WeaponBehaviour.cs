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
    public AttackProgress attackProgress { get; protected set; }
    public AttackProgress bonusAttackProgress { get; protected set; }

    private Coroutine cAttack;
    private Coroutine cBonusAttack;

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

    // Execute an attack with this weapon. Requires the aim direction
    public void Attack(Vector2 aim) =>  cAttack = StartCoroutine(attack(aim));

    // Execute a bonus attack with this weapon. Requires the aim direction
    public void BonusAttack(Vector2 aim) => cBonusAttack = StartCoroutine(attack(aim));

    // Terminates current attack(s).
    public void TerminateAttack() 
    {
        if (cAttack != null)
            StopCoroutine(cAttack); 
        if (cBonusAttack != null)
            StopCoroutine(cBonusAttack);

        attackProgress = AttackProgress.Finished;
        bonusAttackProgress = AttackProgress.Finished;
        StopChargingUp();
    }

    // Invoked when the creature switches to a new weapon. 
    public virtual void ConfigureUponSwitchingToWeapon() 
    {
        attackProgress = AttackProgress.Finished;
        bonusAttackProgress = AttackProgress.Finished;
    }

    // Invoked when a charge-up-fire weapon starts charging up it's attack
    public virtual void StartChargingUp() { }

    // Invoked when a charge-up-fire weapon stops charging it's attack prematurely, before it can fire/execute the attack
    public virtual void StopChargingUp() { }

    // Invoked for a default attack with this weapon. The aim direction is provided
    protected virtual IEnumerator attack(Vector2 aim)
    {
        attackProgress = AttackProgress.Started;
        yield return null;
    }

    // Invoked for a bonus attack with this weapon. The aim direction is specified
    protected virtual IEnumerator bonusAttack(Vector2 aim)
    {
        bonusAttackProgress = AttackProgress.Started;
        yield return null;
    }

    protected void confirmAttackFinished() => attackProgress = AttackProgress.Finished;
    protected void confirmBonusAttackFinished() => bonusAttackProgress = AttackProgress.Finished;

    void Awake()
    {
        phaseTracker = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralPhaseTracker>();
        side = transform.parent.parent.parent.GetComponent<Role>().side;
    }
}