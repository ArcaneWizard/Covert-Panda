using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Shooting : CentralShooting
{
    private AI_LookAround AI_lookAround;
    public Transform player, shootingArm;

    public override Vector2 getAim() => (player.position - shootingArm.position).normalized;

    public override void Awake()
    {
        base.Awake();
        AI_lookAround = transform.GetComponent<AI_LookAround>();
    }

    // Specify how different weapons are fired based on their weapon type
    // and potentially combat mode. Don't fire if the weapon has no ammo, 
    // the last shot/attack isn't finished yet, the player is not in sight 
    // nearby, or the timer in between shots hasn't dropped to zero 
    public override void Update()
    {
        configuration = weaponSystem.weaponConfiguration;
        attackProgress = weaponSystem.IWeapon.attackProgress;

        if (countdownBtwnShots > 0f)
            countdownBtwnShots -= Time.deltaTime;

        if (!AI_lookAround.playerIsInSight || countdownBtwnShots > 0f)
            return;

        if (weaponSystem.GetAmmo <= 0 || attackProgress != "finished")
            return;

        if (configuration.weaponType == Type.singleFire)
        {
            countdownBtwnShots = UnityEngine.Random.Range(0.25f, 0.4f);

            if (combatMode == "gun" || combatMode == "handheld")
                Attack();

            else if (combatMode == "meelee")
                NonAmmoAttack();
        }

        else if (configuration.weaponType == Type.spamFire)
        {
            countdownBtwnShots = 1f / configuration.fireRateInfo;
            Attack();
        }

        else if (configuration.weaponType == Type.chargeUpFire)
            Attack();
    }

    // For "hold-fire" weapons, firing needs to be called every frame.
    // Called by the Late Update method of the AI_LookAround script, 
    // after the weapon rotation is updated over there 
    public override void LateUpdateAfterWeaponRotation()
    {
        if (!AI_lookAround.playerIsInSight)
            return;

        if (weaponSystem.GetAmmo <= 0 || configuration == null)
            return;

        if (configuration.weaponType == Type.holdFire)
            Attack();
    }
}
