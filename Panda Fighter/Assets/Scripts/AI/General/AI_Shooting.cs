using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;

    private Vector2 reactionTime = new Vector2(0.4f, 0.67f);
    private Vector2 angleAimIsOffBy = new Vector2(-12, 12f);
    private float offsetAngle;

    private float countdownBtwnShots = 0f;
    private float timeSinceLastShot = 0f;

    // aim should be off slightly by some random, offset angle
    protected override Vector2 GetAim() 
    {
         offsetAngle = Random.Range(angleAimIsOffBy.x, angleAimIsOffBy.y);
         return Quaternion.AngleAxis(offsetAngle, Vector3.forward) * AI_lookAround.directionToLook.normalized;
    }

    protected override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (health.IsDead)
        {
            countdownBtwnShots = 0f;
            return;
        }

        WeaponConfiguration configuration = weaponSystem.CurrentWeaponConfiguration;
        WeaponBehaviour behaviour = weaponSystem.CurrentWeaponBehaviour;

        if (!AI_lookAround.EnemySpotted || countdownBtwnShots > 0f || weaponSystem.CurrentAmmo <= 0)
            return;

        if (behaviour.attackProgress != AttackProgress.Finished)
            return;

        if (configuration.FiringMode == FiringMode.SingleFire || configuration.FiringMode == FiringMode.SpamFire)
        {
            countdownBtwnShots = 1 / configuration.FireRateInfo + reactionDelay;
            timeSinceLastShot = 0f;
            AttackWithWeapon();
        }
        else if (configuration.FiringMode == FiringMode.ChargeUpFire) 
        {
            countdownBtwnShots = configuration.FireRateInfo + reactionDelay;
            timeSinceLastShot = 0f;
            AttackWithWeapon();
        }

        else 
            AttackWithWeapon();
    }

    private void FixedUpdate()
    {
        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.fixedDeltaTime;
    }

    private float reactionDelay => (timeSinceLastShot > 1f) 
        ? Random.Range(reactionTime.x, reactionTime.y) 
        : Random.Range(0.1f, 0.22f);
}
