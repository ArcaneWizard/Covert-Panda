using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponConfig : MonoBehaviour
{
    //unique for each weapon
    public string combatMode;

    [Header("Required")]
    public List<GameObject> limbs = new List<GameObject>();
    public Transform bulletSpawnPoint;

    [Header("Optional for handheld weapons")]
    public GameObject weapon;
    public Transform aimTarget;
    
    [HideInInspector] public List<Vector2> IK_Coordinates = AimingDir.defaultAiming;
    [HideInInspector] public CentralWeaponSystem weaponSystem;
    [HideInInspector] public Animator animator;

    private Transform entity;

    void Awake() 
    {
        entity = transform.parent.parent.parent.GetChild(0);
        weaponSystem = entity.GetComponent<CentralWeaponSystem>();
        animator = entity.GetComponent<Animator>();

        if (weaponSystem == null || animator == null)
            Debug.LogError("entity's hierarchy may have been changed");
    }
}
