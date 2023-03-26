using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbCollider : MonoBehaviour
{

   /* private void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject.layer == Layers.Weapons) 
        {
            bool pickedup = weaponSystem.PickupWeaponIfInventoryNotFull(col.GetComponent<WeaponTag>().tag);
            col.gameObject.SetActive(false);
            col.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
        }
    }*/
}
