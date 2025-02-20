using System.Collections.Generic;

using MEC;

using UnityEngine;

// THIS CLASS VERSION IS OUTDATED RUSHEEL, OR IF YOU WANT TO CALL IT, DEPRECATED
public class W_FragGrenade : WeaponBehaviour
{
    protected static float grenadeThrowForce = 2200;
    protected static float grenadeYForce = -20;

    // private Vector2 timeB4Release;
    // private Vector2 timeAfterRelease;

    protected override void attack(Vector2 aim)
    {
        base.ConfigureUponPullingOutWeapon();
        Timing.RunSafeCoroutine(configure(aim), gameObject);
    }

    protected IEnumerator<float> configure(Vector2 aim)
    {
        weaponConfiguration.Animator.SetInteger("Arms Phase", 0);
        // timeB4Release.Seconds = CommonWeaponBehaviours.CalculateTimeB4ReleasingGrenade(0.02f, 0.2f, aim);
        // timeAfterRelease.Seconds = 0.6f - timeB4Release.Seconds;

        yield return Timing.WaitForSeconds(2f);
    }

    /*
    protected override void startMultiActionAttack(bool singleAction)
    {
        attackTimes = new List<ExecutionDelay>() { ExecutionDelay.Instant, timeB4Release, timeAfterRelease };
        attackActions = new List<Action>() { getTimeB4Release, throwGrenade, disableArms};

        base.startMultiActionAttack(false);

        void getTimeB4Release()
        {
            timeB4Release.Seconds = WeaponBehaviourHelper.CalculateTimeB4ReleasingGrenade(0.02f, 0.2f, aim);
            timeAfterRelease.Seconds = 0.6f - timeB4Release.Seconds;
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
    }*/
}
