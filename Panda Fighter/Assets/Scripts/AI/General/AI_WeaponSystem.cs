
using System.Collections;
using UnityEngine;
using System.Linq;

public class AI_WeaponSystem : CentralWeaponSystem
{
    private bool feelsLikePickingUpWeapon;

    private const float timeB4MoodChange = 4f;
    private const float percentChanceOfPickingUpWeapon = 40;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(updateMood());
    }

    // every few seconds, the creature makes its mind whether it feels like picking up weapons or not
    private IEnumerator updateMood()
    {
        yield return new WaitForSeconds(timeB4MoodChange);
        feelsLikePickingUpWeapon = Random.Range(0, 100) < percentChanceOfPickingUpWeapon;
    }

    void Update()
    {
        if (Health.IsDead)
            return;

        // switch to another inventory weapon if the ammo is 0
        if (CurrentAmmo == 0)
        {
            foreach (Weapon weapon in inventoryWeapons.Keys)
            {
                if (CurrentWeapon != weapon)
                {
                    int slot = inventoryWeapons[weapon];
                    switchWeapons(slot);
                }
            }
        }
    }

    // pick up a weapon only if the creature feels like doing so
    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == Layer.PickableWeapons && feelsLikePickingUpWeapon)
        {
            Weapon weapon = col.transform.GetComponent<WeaponTag>().Tag;
            pickupWeaponIntoCurrentSlot(weapon);
            col.gameObject.SetActive(false);
            col.transform.parent.GetComponent<SpawnRandomWeapon>().startCountdownForNewWeapon();
        }
    }
}

