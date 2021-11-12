using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    private float countdownBtwnShots = 0f;

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

        if (!AI_lookAround.playerIsInSight || countdownBtwnShots > 0f || weaponSystem.weaponSelected == null)
            return;

        if (weaponSystem.getAmmo() > 0 && weaponSystem.getWeapon().tag == "singleFire")
        {
            if (combatMode == "gun")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "handheld" && weaponSystem.getWeaponConfig().attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                Attack();
            }

            else if (combatMode == "meelee" && weaponSystem.getWeaponConfig().attackProgress == "finished")
            {
                countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.33f);
                MeeleeAttack();
            }
        }

        if (weaponSystem.getAmmo() > 0 && combatMode == "gun" && weaponSystem.getWeapon().tag == "spamFire")
        {
            countdownBtwnShots = 1f / weaponSystem.getWeaponConfig().config.ratePerSecond;
            Attack();
        }
    }
}
