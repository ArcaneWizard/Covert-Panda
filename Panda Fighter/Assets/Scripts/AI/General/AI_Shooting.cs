using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;

    private Vector2 reactionTime = new Vector2(0.4f, 0.67f);
    private Vector2 angleAimIsOffBy = new Vector2(-12, 12f);

    private float timeBtwnShots = 0f;
    private float timeSinceLastShot = 0f;
    private bool isCharged;

    public override void ConfigureUponPullingOutWeapon() => reset();

    // aim should be off slightly by a random offset angle
    protected override Vector2 GetAim() 
    {
         float offsetAngle = Random.Range(angleAimIsOffBy.x, angleAimIsOffBy.y);
         return Quaternion.AngleAxis(offsetAngle, Vector3.forward) * AI_lookAround.directionToLook.normalized;
    }

    protected override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    private void reset()
    {
        timeBtwnShots = 0f;
    }


    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (health.IsDead)
        {
            reset();
            return;
        }

        var configuration = weaponSystem.CurrentWeaponConfiguration;
        var behaviour = weaponSystem.CurrentWeaponBehaviour;

        if (!AI_lookAround.EnemySpotted || timeBtwnShots > 0f || weaponSystem.CurrentAmmo <= 0)
            return;

        if (behaviour.attackProgress != AttackProgress.Finished)
            return;

        if (configuration.FiringMode == FiringMode.SingleFire || configuration.FiringMode == FiringMode.SpamFire)
        {
            timeBtwnShots = 1 / configuration.FireRateInfo + reactionDelay;
            timeSinceLastShot = 0f;
            AttackWithWeapon();
        }
        else if (configuration.FiringMode == FiringMode.ChargeUpFire)
        {
          // To-DO
          // AI charges up its weapon when it spots an enemy, but also sometimes before spotting an enemy
          // AI should be smart about when it starts charging or lets go off charging
        }

        else
        {
            timeSinceLastShot = 0f;
            AttackWithWeapon();
        }
    }

    private void FixedUpdate()
    {
        if (timeBtwnShots > 0f)
            timeBtwnShots -= Time.fixedDeltaTime;
    }

    private float reactionDelay => (timeSinceLastShot > 1f) 
        ? Random.Range(reactionTime.x, reactionTime.y) 
        : Random.Range(0.1f, 0.22f);
}
