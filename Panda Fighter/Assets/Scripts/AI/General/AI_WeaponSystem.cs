
using System.Collections;

using UnityEngine;

public class AI_WeaponSystem : CentralWeaponSystem
{
    private bool feelsLikePickingUpWeapon;

    private const float TIME_B4_MOOD_CHANGE = 4f;
    private const float PERCENT_CHANCE_OF_PICKING_UP_WEAPON = 40;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(updateMood());
    }

    // every few seconds, the creature makes its mind whether it feels like picking up weapons or not
    private IEnumerator updateMood()
    {
        yield return new WaitForSeconds(TIME_B4_MOOD_CHANGE);
        feelsLikePickingUpWeapon = Random.Range(0, 100) < PERCENT_CHANCE_OF_PICKING_UP_WEAPON;
    }

    void Update()
    {
        if (health.IsDead)
            return;

        // switch to another inventory weapon if the ammo is 0
        if (CurrentAmmo == 0) {
            foreach (Weapon weapon in inventoryWeapons.Keys) {
                if (CurrentWeapon != weapon) {
                    int slot = inventoryWeapons[weapon];
                    switchWeapons(slot);
                }
            }
        }
    }

    // pick up a weapon only if the creature feels like doing so
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.PickableWeapons && feelsLikePickingUpWeapon) {
            Weapon weapon = col.transform.GetComponent<WeaponTag>().Tag;
            pickupWeaponIntoCurrentSlot(weapon);
            col.gameObject.SetActive(false);
            col.transform.parent.GetComponent<SpawnRandomWeapon>().StartCountdownForNewWeapon();
        }
    }
}

