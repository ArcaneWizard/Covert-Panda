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

    [HideInInspector] public string attackProgress { get; private set; }
    [HideInInspector] public WeaponConfig config;

    public void DoSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(SetupAttack(aim, bullet, rig));
    }

    public void DoAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        Attack(aim, bullet, rig);
        config.shooting.updateWeaponHeldForHandheldWeapons();
        attackProgress = "finished";
    }

    public void DoBonusSetupAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        attackProgress = "started";
        StartCoroutine(BonusSetupAttack(aim, bullet, rig));
    }

    public void DoBonusAttack(Vector2 aim, Transform bullet, Rigidbody2D rig)
    {
        BonusAttack(aim, bullet, rig);
        attackProgress = "finished";
    }

    public virtual void Awake()
    {
        config = transform.GetComponent<WeaponConfig>();
        attackProgress = "finished";
    }
}


