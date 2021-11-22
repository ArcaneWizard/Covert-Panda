using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    private float countdownBtwnShots = 0f;
    private WeaponConfiguration configuration;

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

        if (weaponSystem.GetAmmo <= 0 || weaponSystem.weaponSelected == null)
            return;

        configuration = weaponSystem.weaponConfiguration;
        if (weaponSystem.GetAmmo > 0 && configuration.weaponType == Type.singleFire)
        {
            if (combatMode == "gun")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "handheld" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "meelee" && weaponSystem.IWeapon.attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                NonAmmoAttack();
            }
        }

        if (weaponSystem.GetAmmo > 0 && combatMode == "gun" && configuration.weaponType == Type.spamFire)
        {
            countdownBtwnShots = 1f / weaponSystem.IWeapon.configuration.fireRateInfo;
            Attack();
        }
    }
}
