using UnityEngine;
public class Shooting : CentralShooting
{
    private float timerBtwnShots = 0f;
    private float chargeUpTimer = 0f;
    private bool canExecuteChargedAttack = true;

    public override void ConfigureUponPullingOutWeapon() => reset();
    protected override Vector2 GetAim() => lookAround.directionToLook;

    private void reset()
    {
        timerBtwnShots = 0f;
        chargeUpTimer = 0f;
        canExecuteChargedAttack = true;
    }

    private void Update()
    {
        if (health.IsDead)
        {
            reset();
            return;
        }

        var config = weaponSystem.CurrentWeaponConfiguration;
        var wb = weaponSystem.CurrentWeaponBehaviour;

        if (config.FiringMode != FiringMode.ChargeUpFire)
            canExecuteChargedAttack = true;

        if (weaponSystem.CurrentAmmo <= 0 || wb.attackProgress != AttackProgress.Finished)
            return;

        if (config.FiringMode == FiringMode.SingleFire && Input.GetMouseButtonDown(0) && timerBtwnShots <= 0f)
        {
            timerBtwnShots = 1 / config.FireRateInfo;
            AttackWithWeapon();
        }

        else if (config.FiringMode == FiringMode.SpamFire && Input.GetMouseButton(0) && timerBtwnShots <= 0f)
        {
            timerBtwnShots = 1 / config.FireRateInfo;
            AttackWithWeapon();
        }

        else if (config.FiringMode == FiringMode.ChargeUpFire)
        {
            // set the charge up timer when the right mouse button is first pressed
            if (Input.GetMouseButtonDown(0))
            {
                chargeUpTimer = config.FireRateInfo;
                wb.StartChargingUp();
            }

            // tick down the timer whlie the right mouse button is held
            if (Input.GetMouseButton(0) && chargeUpTimer > 0f)
                chargeUpTimer -= Time.deltaTime;

            // once the the timer is up, shoot with the weapon if it hasn't already shot
            if (Input.GetMouseButton(0) && chargeUpTimer <= 0f && canExecuteChargedAttack)
            {
                AttackWithWeapon();
                canExecuteChargedAttack = false;
            }

            // once the mouse button is let go, the weapon is allowed to charge up again 
            if (!canExecuteChargedAttack && Input.GetMouseButtonUp(0))
                canExecuteChargedAttack = true;

            if (Input.GetMouseButtonUp(0))
                wb.StopChargingUp();
        }

        else if (config.FiringMode == FiringMode.ContinousBeam && Input.GetMouseButton(0))
            AttackWithWeapon();
    }

    private void FixedUpdate()
    {
        if (timerBtwnShots > 0f)
            timerBtwnShots -= Time.fixedDeltaTime;
    }
}
