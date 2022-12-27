using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponMechanics : MonoBehaviour
{
    public virtual void SetDefaultAnimation() { return; }
    public abstract IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public abstract void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public virtual IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { yield return null; }
    public virtual void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { return; }

    public string attackProgress { get; protected set; }
    public string bonusAttackProgress { get; protected set; }
    public CentralShooting shooting { get; protected set; }
    public CentralPhaseTracker phaseTracker { get; protected set; }
    public Side side { get; protected set; }
    
    protected WeaponConfiguration config;

    public void Initialize(WeaponConfiguration config) => this.config = config;

    public void DoSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(SetupAttack(aim, bullet, rig));
    }

    public void DoAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        Attack(aim, bullet, rig);
        config.shooting.LetGoOffAnyGrenades();
        attackProgress = "finished";
    }

    public void DoBonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        bonusAttackProgress = "started";
        StartCoroutine(BonusSetupAttack(aim, bullet, rig));
    }

    public void DoBonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        BonusAttack(aim, bullet, rig);
        bonusAttackProgress = "finished";
    }

    //referenced when you switch to a different weapons
    public void resetAttackProgress()
    {
        attackProgress = "finished";
        bonusAttackProgress = "finished";
    }

    void Awake()
    {
        shooting = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralShooting>();
        phaseTracker = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralPhaseTracker>();
        side = transform.parent.parent.parent.GetComponent<Role>().side;

        attackProgress = "finished";
        bonusAttackProgress = "finished";
    }
}


