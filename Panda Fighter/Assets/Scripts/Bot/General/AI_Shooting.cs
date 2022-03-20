using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    private float countdownBtwnShots = 0f;
    private WeaponConfiguration configuration;

    private Vector2 reactionTime = new Vector2(0.2f, 0.35f);

    public override Vector2 getAim() => AI_lookAround.lookAt.normalized;

    public override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    void Update()
    {
        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (health.isDead || !AI_lookAround.playerIsInSight || countdownBtwnShots > 0f)
            return;

        if (weaponSystem.GetAmmo <= 0 || weaponSystem.weaponSelected == null)
            return;

        configuration = weaponSystem.weaponConfiguration;
        if (weaponSystem.GetAmmo > 0 && configuration.weaponType == Type.singleFire)
        {
            if (combatMode == "gun")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                Attack();
            }

            else if (combatMode == "handheld" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                Attack();
            }

            else if (combatMode == "meelee" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = configuration.fireRateInfo + reactionDelay;
                NonAmmoAttack();
            }
        }

        if (weaponSystem.GetAmmo > 0 && combatMode == "gun" && configuration.weaponType == Type.spamFire)
        {
            countdownBtwnShots = configuration.fireRateInfo;
            Attack();
        }
    }

    public void LateLateUpdate()
    {
        if (health.isDead || !AI_lookAround.playerIsInSight || weaponSystem.GetAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire)
            Attack();
    }

    private float reactionDelay => UnityEngine.Random.Range(reactionTime.x, reactionTime.y);
}
