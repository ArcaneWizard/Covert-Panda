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
    public override Vector2 GetAim() 
    {
         offsetAngle = UnityEngine.Random.Range(angleAimIsOffBy.x, angleAimIsOffBy.y);
         return Quaternion.AngleAxis(offsetAngle, Vector3.forward) * AI_lookAround.directionToLook.normalized;
    }

    protected override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (health.isDead)
        {
            countdownBtwnShots = 0f;
            return;
        }

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        WeaponConfiguration configuration = weaponSystem.CurrentWeaponConfiguration;
        WeaponBehaviour implementation = weaponSystem.CurrentWeaponImplementation;

        if (!AI_lookAround.EnemySpotted || countdownBtwnShots > 0f || weaponSystem.CurrentAmmo <= 0)
            return;

        if (implementation.attackProgress == Progress.Finished)
            return;

        if (configuration.WeaponType != FiringModes.holdFire)
        {
            countdownBtwnShots = configuration.FireRateInfo + reactionDelay;
            timeSinceLastShot = 0f;
            AttackWithWeapon();
        }
        else 
            AttackWithWeapon();
    }

    private float reactionDelay => (timeSinceLastShot > 1f) 
        ? UnityEngine.Random.Range(reactionTime.x, reactionTime.y) 
        : UnityEngine.Random.Range(0.1f, 0.22f);
}
