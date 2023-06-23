using System;
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

    private TimedCode attackTimedActions;
    private TimedCode bonusAttackTimedActions;

    protected List<ExecutionDelay> attackTimes;
    protected List<ExecutionDelay> bonusAttackTimes;
    protected List<Action> attackActions;
    protected List<Action> bonusAttackActions;

    protected Vector2 aim;
    protected Vector2 bonusAim;

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

        attackTimes = new List<ExecutionDelay>() { };
        bonusAttackTimes = new List<ExecutionDelay>() { };
        attackActions = new List<Action>() { };
        bonusAttackActions = new List<Action>() { };
    }

    private void Start()
    {
        startMultiActionAttack(true);
        startMultiActionBonusAttack(true);
    }

    // Override to implement a single action attack
    protected virtual void startAttack() { }

    // Override to implement a single action bonus attack
    protected virtual void startBonusAttack() { }

    // Construct a multi or single action attack
    protected virtual void startMultiActionAttack(bool singleAction)
    {
        if (singleAction)
        {
            attackTimes = new List<ExecutionDelay>() { ExecutionDelay.Instant };
            attackActions = new List<Action>() { startAttack };
        }

        else if (attackTimes.Count == 0 || attackTimes.Count != attackActions.Count)
            Debug.Log("Invalid Attack Specified");

        attackTimes.Add(ExecutionDelay.Instant);
        attackActions.Add(confirmAttackFinished);
        attackTimedActions = new TimedCode(attackTimes, attackActions);
    }

    // Construct a multi or single action bonus attack
    protected virtual void startMultiActionBonusAttack(bool singleAction)
    {
        if (singleAction)
        {
            bonusAttackTimes = new List<ExecutionDelay>() { ExecutionDelay.Instant };
            bonusAttackActions = new List<Action>() { startBonusAttack };
        }

        else if (bonusAttackTimes.Count == 0 || bonusAttackTimes.Count != bonusAttackActions.Count)
            Debug.Log("Invalid Bonus Attack Specified");

        bonusAttackTimes.Add(ExecutionDelay.Instant);
        bonusAttackActions.Add(confirmBonusAttackFinished);
        bonusAttackTimedActions = new TimedCode(bonusAttackTimes, bonusAttackActions);
    }

    // Execute an attack with this weapon. Requires the aim direction
    public void Attack(Vector2 aim)
    {
        this.aim = aim;
        attackTimedActions.Start();
    }

    // Execute a bonus attack with this weapon. Requires the aim direction
    public void BonusAttack(Vector2 aim)
    {
        this.bonusAim = aim;
        bonusAttackTimedActions.Start();
    } 

    // Terminates current attack(s).
    public void TerminateAttackEarly() 
    {
        attackTimedActions?.StopEarly();
        bonusAttackTimedActions?.StopEarly();

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

    void Update()
    {
        attackTimedActions?.Update();
        bonusAttackTimedActions?.Update();
    }
 }