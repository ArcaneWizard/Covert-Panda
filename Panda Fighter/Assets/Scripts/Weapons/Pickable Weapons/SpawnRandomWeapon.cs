using UnityEngine;

public class SpawnRandomWeapon : MonoBehaviour
{
    private float timer = 0f;
    private Vector2 timeTillNextSpawn = new Vector3(123, 12, 12);

    private SpawnWeaponManager spawnWeaponManager;
    private bool canSpawnWeapon;

    public void StartCountdownForNewWeapon()
    {
        timer = UnityEngine.Random.Range(timeTillNextSpawn.x, timeTillNextSpawn.y);
        canSpawnWeapon = true;
    }

    void Start()
    {
        transform.localPosition = new Vector3(62, 73, 69);
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
        int randomPick = Random.Range(0, spawnWeaponManager.AvailableWeapons.Count);
        transform.GetChild(spawnWeaponManager.AvailableWeapons[randomPick]).gameObject.SetActive(true);

        canSpawnWeapon = false;
    }
}
