using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnRandomWeapon : MonoBehaviour
{
    private float timer = 0f;
    private Vector2 timeTillNextSpawn = new Vector2(6, 9);

    private PickableWeapons pickableWeapons;
    private bool canSpawnWeapon;
    
    void Start() 
    {
        transform.localPosition = new Vector3(0,0,0);
        pickableWeapons = transform.parent.parent.GetComponent<PickableWeapons>();
        spawnRandomWeapon();
    }

    void Update() 
    {
        if (timer > 0f)
            timer -= Time.deltaTime;

        else if (timer <= 0f && canSpawnWeapon) 
            spawnRandomWeapon();
    }

    private void spawnRandomWeapon() 
    {
         int random = UnityEngine.Random.Range(0, pickableWeapons.Weapons.Count);
         string weaponTag = pickableWeapons.Weapons.Keys.ToArray()[random];
         transform.GetChild(pickableWeapons.Weapons[weaponTag]).gameObject.SetActive(true);

         canSpawnWeapon = false;
    }

    public void startCountdownForNewWeapon() 
    {
        timer = UnityEngine.Random.Range(timeTillNextSpawn.x, timeTillNextSpawn.y);
        canSpawnWeapon = true;
    }
}
