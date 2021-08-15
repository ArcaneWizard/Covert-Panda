using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralShooting : MonoBehaviour
{
    [HideInInspector]
    public GameObject newWeaponHeld = null;
    public string combatMode = "gun";

    protected CentralWeaponSystem weaponSystem;
    private CentralLookAround lookAround;

    private List<GameObject> WeaponSetup = new List<GameObject>();
    private Transform bulletSpawnPoint;

    public void Awake()
    {
        weaponSystem = transform.GetComponent<CentralWeaponSystem>();
        lookAround = transform.GetComponent<CentralLookAround>();
    }

    private void LateUpdate()
    {
        if (combatMode == "handheld")
        {
            if (weaponSystem.getAmmo() > 0)
            {
                newWeaponHeld.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                newWeaponHeld.transform.GetComponent<Collider2D>().isTrigger = true;

                newWeaponHeld.transform.position = bulletSpawnPoint.position;
                newWeaponHeld.transform.rotation = bulletSpawnPoint.rotation;
                newWeaponHeld.SetActive(true);
            }
        }
    }

    //specify which limbs, weapon and aim target to activate (the latter helps a weapon track while aiming) 
    public void configureWeaponAndArms()
    {
        //deactivate the previous animated arm limbs + weapon
        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(false);
        WeaponSetup.Clear();

        IWeapon Iweapon = weaponSystem.getWeaponConfig();

        lookAround.setAimTarget(Iweapon.config.aimTarget);
        bulletSpawnPoint = Iweapon.config.bulletSpawnPoint;
        
        if (Iweapon.config.weapon != null)
            WeaponSetup.Add(Iweapon.config.weapon);
        foreach (GameObject limb in Iweapon.config.limbs)
            WeaponSetup.Add(limb);

        foreach (GameObject limb_Or_Weapon in WeaponSetup)
            limb_Or_Weapon.SetActive(true);

        List<Vector2> aiming = Iweapon.config.IK_Coordinates;
        lookAround.calculateShoulderAngles(aiming);
    }
}
