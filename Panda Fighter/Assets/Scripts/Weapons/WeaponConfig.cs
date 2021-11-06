using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    [Header("Key Info")]
    public string combatMode;
    public int ammoWhenEquipped;

    [Header("Required")]
    public List<GameObject> limbs = new List<GameObject>();
    public Transform bulletSpawnPoint;

    [Header("Optional for handheld weapons")]
    public GameObject weapon;
    public Transform aimTarget;
    
    [HideInInspector] public List<Vector2> IK_Coordinates = AimingDir.defaultAiming;
    [HideInInspector] public CentralWeaponSystem weaponSystem;
    [HideInInspector] public CentralShooting shooting;
    [HideInInspector] public Animator animator;

    private Transform entity;

    void Awake() 
    {
        entity = transform.parent.parent.parent.transform.GetChild(0);
        weaponSystem = entity.GetComponent<CentralWeaponSystem>();
        shooting = entity.GetComponent<CentralShooting>();
        animator = entity.GetComponent<Animator>();
    }
}
