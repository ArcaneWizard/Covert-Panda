using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class wFragGrenade : WeaponBehaviour
{
    protected static float grenadeThrowForce = 2200;
    protected static float grenadeYForce = -20;

    private ExecutionDelay timeB4Release = ExecutionDelay.Unknown;
    private ExecutionDelay timeAfterRelease = ExecutionDelay.Unknown;

    public override void ConfigureUponPullingOutWeapon()
    {
        base.ConfigureUponPullingOutWeapon();
        weaponConfiguration.Animator.SetInteger("Arms Phase", 0);
    }

    protected override void startMultiActionAttack(bool singleAction)
    {
        attackTimes = new List<ExecutionDelay>() { ExecutionDelay.Instant, timeB4Release, timeAfterRelease };
        attackActions = new List<Action>() { getTimeB4Release, throwGrenade, disableArms};

        base.startMultiActionAttack(false);

        void getTimeB4Release()
        {
            timeB4Release.seconds = WeaponBehaviourHelper.CalculateTimeB4ReleasingGrenade(0.02f, 0.2f, aim);
            timeAfterRelease.seconds = 0.6f - timeB4Release.seconds;
            //weaponConfiguration.Arms[1].transform.parent.GetComponent<Animator>().Play(0, -1);
            //weaponConfiguration.Arms[1].SetActive(true);
        }

        void throwGrenade()
        {
            Transform grenade = null;// WeaponBehaviourHelper.SpawnGrenade(aim, grenadeSystem, weaponConfiguration, side, false);
            grenade.transform.right = -aim;

            grenade.GetComponent<Collider2D>().isTrigger = false;
            grenade.GetComponent<FragGrenade>().startExplosionTimer();

            Rigidbody2D rig = grenade.GetComponent<Rigidbody2D>();
            Vector2 unadjustedForce = weaponConfiguration.Speed * 40 * aim * new Vector2(1.2f, 1) + new Vector2(0, grenadeYForce);
            rig.AddForce(unadjustedForce * rig.mass);
        }

        void disableArms()
        {
            //weaponConfiguration.Arms[1].SetActive(false);
        }
    }
}
