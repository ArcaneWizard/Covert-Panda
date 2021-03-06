using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWeapon : MonoBehaviour
{
    public virtual void SetDefaultAnimation() { return; }
    public abstract IEnumerator SetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public abstract void Attack(Vector2 aim, Transform bullet, Rigidbody2D rig);
    public virtual IEnumerator BonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { yield return null; }
    public virtual void BonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig) { return; }

    public string attackProgress { get; protected set; }
    public string bonusAttackProgress { get; protected set; }
    public CentralShooting shooting { get; protected set; }
    public CentralAnimationController animController { get; protected set; }
    public Side side { get; protected set; }

    [HideInInspector] public WeaponConfiguration configuration;

    public void DoSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(SetupAttack(aim, bullet, rig));
    }

    public void DoAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        Attack(aim, bullet, rig);
        configuration.shooting.LetGoOffAnyGrenades();
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

    public virtual void Awake()
    {
        shooting = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralShooting>();
        animController = transform.parent.parent.parent.transform.GetChild(0).transform.GetComponent<CentralAnimationController>();
        side = transform.parent.parent.parent.GetComponent<Role>().side;

        attackProgress = "finished";
        bonusAttackProgress = "finished";
    }

    //referenced when you switch to a different weapons
    public void resetAttackProgress()
    {
        attackProgress = "finished";
        bonusAttackProgress = "finished";
    }
}


