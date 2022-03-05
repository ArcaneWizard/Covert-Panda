
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Health : Health
{
    public override void Awake()
    {
        base.Awake();

        maxHP = 300;
        bulletLayer = Layers.friendlyBullet;
    }

    public override IEnumerator CallUponDying() 
    {
        yield return new WaitForSeconds(respawnTime);
        currentHP = maxHP;
        isDead = false;

        ragdolling.Disable();
        weaponSystem.InitializeWeaponSystem();

        Transform respawnLocation = respawnLocations.GetChild(
            UnityEngine.Random.Range(0,respawnLocations.childCount));
        transform.position = respawnLocation.position;  

        yield return new WaitForSeconds(Time.deltaTime);
        hpBar.transform.parent.gameObject.SetActive(true);
        controller.updateGroundAngle(false);
        controller.forceUpdateTilt = true;

        yield return new WaitForSeconds(respawnInvulnerabilityDuration);
        hitBox.enabled = true;
    }
}
