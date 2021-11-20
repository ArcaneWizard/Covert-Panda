using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    private float countdownBtwnShots = 0f;
    private WeaponConfig config;

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

        if (!AI_lookAround.playerIsInSight || countdownBtwnShots > 0f)
            return;

        if (weaponSystem.getAmmo <= 0 || weaponSystem.weaponSelected == null)
            return;

        config = weaponSystem.weaponConfig;
        if (weaponSystem.getAmmo > 0 && config.weaponType == Type.singleFire)
        {
            if (combatMode == "gun")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "handheld" && weaponSystem.weapon.attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "meelee" && weaponSystem.weapon.attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                NonAmmoAttack();
            }
        }

        if (weaponSystem.getAmmo > 0 && combatMode == "gun" && config.weaponType == Type.spamFire)
        {
            countdownBtwnShots = 1f / weaponSystem.weapon.config.fireRateInfo;
            Attack();
        }
    }
}
