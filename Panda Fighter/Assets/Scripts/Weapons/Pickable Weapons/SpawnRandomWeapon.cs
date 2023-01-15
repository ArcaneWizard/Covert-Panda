using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnRandomWeapon : MonoBehaviour
{
    private float timer = 0f;
    private Vector2 timeTillNextSpawn = new Vector2(6, 9);

    private SpawnWeaponManager spawnWeaponManager;
    private bool canSpawnWeapon;
    
    void Start() 
    {
        transform.localPosition = new Vector3(0,0,0);
        spawnWeaponManager = transform.parent.parent.GetComponent<SpawnWeaponManager>();
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
         int randomPick = UnityEngine.Random.Range(0, spawnWeaponManager.AvailableWeapons.Count);
         transform.GetChild(spawnWeaponManager.AvailableWeapons[randomPick]).gameObject.SetActive(true);

         canSpawnWeapon = false;
    }

    public void startCountdownForNewWeapon() 
    {
        timer = UnityEngine.Random.Range(timeTillNextSpawn.x, timeTillNextSpawn.y);
        canSpawnWeapon = true;
    }
}
