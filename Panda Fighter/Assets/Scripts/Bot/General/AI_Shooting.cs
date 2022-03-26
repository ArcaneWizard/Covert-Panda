using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    private WeaponConfiguration configuration;

    private Vector2 reactionTime = new Vector2(0.4f, 0.67f);
    private Vector2 angleAimIsOffBy = new Vector2(-30, 30f);
    private float offsetAngle;

    private float countdownBtwnShots = 0f;
    private float timeSinceLastShot = 0f;

    // aim should be off slightly by some random, offset angle
    public override Vector2 getAim() 
    {
         offsetAngle = UnityEngine.Random.Range(angleAimIsOffBy.x, angleAimIsOffBy.y);
         return Quaternion.AngleAxis(offsetAngle, Vector3.forward) * AI_lookAround.lookAt.normalized;
    }

    public override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (health.isDead || !AI_lookAround.targetInSight || countdownBtwnShots > 0f)
            return;

        if (weaponSystem.GetAmmo <= 0 || weaponSystem.weaponSelected == null)
            return;

        configuration = weaponSystem.weaponConfiguration;
        if (weaponSystem.GetAmmo > 0 && configuration.weaponType == Type.singleFire)
        {
            if (combatMode == "gun")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                timeSinceLastShot = 0f;
                Attack();
            }

            else if (combatMode == "handheld" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                timeSinceLastShot = 0f;
                Attack();
            }

            else if (combatMode == "meelee" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                timeSinceLastShot = 0f;
                NonAmmoAttack();
            }
        }

        if (weaponSystem.GetAmmo > 0 && combatMode == "gun" && configuration.weaponType == Type.spamFire)
        {
            countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
            timeSinceLastShot = 0f;
            Attack();
        }
    }

    public void LateLateUpdate()
    {
        if (health.isDead || !AI_lookAround.targetInSight || weaponSystem.GetAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire)
            Attack();
    }

    private float reactionDelay => (timeSinceLastShot > 1f) ? UnityEngine.Random.Range(reactionTime.x, reactionTime.y) : 0;
}
