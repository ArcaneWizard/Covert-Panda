using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public RespawningText respawnText;
    public GameObject inventory;
    public override void Awake()
    {
        base.Awake();

        maxHP = 400;
        bulletLayer = Layers.enemyBullet;
    }

    public override IEnumerator CallUponDying() 
    {
        inventory.SetActive(false);
        respawnText.StartRespawnCountdown(respawnTime);

        yield return new WaitForSeconds(respawnTime);
        currentHP = maxHP;
        isDead = false;

        ragdolling.Disable();
        inventory.SetActive(true);
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
