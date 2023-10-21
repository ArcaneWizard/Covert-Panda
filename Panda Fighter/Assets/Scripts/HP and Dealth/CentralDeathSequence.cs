using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

///<summary> Handles what occurs from the moment the player dies to the moment after they respawn. </summary>
public class CentralDeathSequence : MonoBehaviour
{
    protected const float respawnTime = 4.22f;
    private Transform respawnLocations;

    public Action UponDying;
    public Action RightBeforeRespawning;
    public Action UponRespawning;

    private Side side;
    private CentralWeaponSystem weaponSystem;
    protected CentralController controller;
    protected CentralAbilityHandler abilityHandler;
    protected Ragdolling ragdolling;

    protected virtual void Awake()
    {
        controller = transform.GetComponent<CentralController>();
        abilityHandler = transform.GetComponent<CentralAbilityHandler>();
        side = transform.parent.GetComponent<Role>().side;
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        ragdolling = transform.GetComponent<Ragdolling>();

        respawnLocations = (side == Side.Friendly)
            ? transform.parent.parent.parent.GetComponent<References>().FriendRespawnPoints
            : transform.parent.parent.parent.GetComponent<References>().EnemyRespawnPoints;
    }

    // Trigger the death sequence in 3 seperate stages 
    // 1) Immediate actions as the player has just died
    // 2) Actions when the player is ready to respawn
    // 3) Actions after the player has respawned on the map
    public IEnumerator Initiate()
    {
        uponDying();
        UponDying?.Invoke();

        yield return new WaitForSeconds(respawnTime);
        RightBeforeRespawning?.Invoke();
        rightBeforeRespawning();

        yield return new WaitForSeconds(Time.deltaTime);
        UponRespawning?.Invoke();
        uponRespawning();
    }

    protected virtual void uponDying()
    {
        ragdolling.EnableRagdolling();
        Stats.ConfirmDeathFor(transform.parent);
    }

    protected virtual void rightBeforeRespawning()
    {
        ragdolling.DisableRagdolling();
        weaponSystem.ResetInventory();

        Transform respawnLocation = respawnLocations.GetChild(
            UnityEngine.Random.Range(0, respawnLocations.childCount));

        transform.position = new Vector3(respawnLocation.position.x, respawnLocation.position.y, 0);
        transform.localEulerAngles = new Vector3(0, 0, 0);
        controller.UpdateTiltInstantly();
    }

    protected virtual void uponRespawning()
    {
        StartCoroutine(abilityHandler.EnableSpawnProtection());
    }
}