using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

// This abstract class is utilized to implement a weapon's behaviour:
// aka how it fires/attacks, and any animation/behaviour the moment its equipped.
// The progress of an "attack" is tracked, useful to know for weapons
// that charge up or meelee weapons that swing. Additionally, weapons
// can implement a 2nd/bonus attack.

public abstract class WeaponBehaviour : MonoBehaviour
{
    public AttackProgress attackProgress { get; protected set; }
    public AttackProgress bonusAttackProgress { get; protected set; }

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

        attackProgress = AttackProgress.Finished;
        bonusAttackProgress = AttackProgress.Finished;
    }

    protected abstract void attack(Vector2 aim);
    protected virtual void bonusAttack(Vector2 aim) { }

    // Execute an attack with this weapon. Requires the aim direction
    public void Attack(Vector2 aim)
    {
        attackProgress = AttackProgress.Started;
        attack(aim);
    }

    // Execute a bonus attack with this weapon. Requires the aim direction
    public virtual void BonusAttack(Vector2 aim) 
    {
        bonusAttackProgress = AttackProgress.Started;
        bonusAttack(aim);
    }

    // Terminates current attack(s).
    public void TerminateAttackEarly() 
    {
        attackProgress = AttackProgress.Finished;
        bonusAttackProgress = AttackProgress.Finished;
        StopChargingUp();
    }

    // Invoked when the creature switches to a new weapon. 
    public virtual void ConfigureUponPullingOutWeapon() 
    {
        attackProgress = AttackProgress.Finished;
        bonusAttackProgress = AttackProgress.Finished;
    }

    // Invoked when a charge-up-fire weapon starts charging up it's attack
    public virtual void StartChargingUp() { }

    // Invoked when a charge-up-fire weapon stops charging it's attack prematurely, before it can fire/execute the attack
    public virtual void StopChargingUp() { }

    protected void confirmAttackFinished() => attackProgress = AttackProgress.Finished;
    protected void confirmBonusAttackFinished() => bonusAttackProgress = AttackProgress.Finished;

    void Awake()
    {
        phaseTracker = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralPhaseTracker>();
        side = transform.parent.parent.parent.GetComponent<Role>().side;
    }
 }