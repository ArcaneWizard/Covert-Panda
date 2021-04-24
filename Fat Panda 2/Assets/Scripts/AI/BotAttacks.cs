using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAttacks : MonoBehaviour
{
    public GameObject[] bullets;
    public Transform gun;
    public Transform bulletSpawnPoint;

    private Transform bullet;
    private int cycle = 0;
    private float bulletSpeed = 20;

    private float cooldownTimer = 0;
    private float cooldownTime = 0.1f;

    void Update()
    {
        //cooldown Timer ticks down
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    public void shootBullet(Vector2 dir)
    {
        //if the shooting cooldown is over
        if (cooldownTimer <= 0f)
        {
            //shoot the bullet
            bullet = bullets[cycle].transform;
            bullet.transform.position = bulletSpawnPoint.position;
            bullet.gameObject.SetActive(true);
            bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;

            //cycle to a different bullet in the array next time
            cycle = ++cycle % bullets.Length;
            cooldownTimer = cooldownTime;
        }
    }
}






