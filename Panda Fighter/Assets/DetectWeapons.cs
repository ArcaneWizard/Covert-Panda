using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectWeapons : MonoBehaviour
{
    public CentralWeaponSystem weaponSystem;

    private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.layer == Layers.pickableWeapon) 
        {
            weaponSystem.collectNewWeapon(col.GetComponent<WeaponTag>().Tag);
            col.gameObject.SetActive(false);
            col.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
        }
    }
}
