using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAttacks : MonoBehaviour
{
    private List<GameObject> bullets = new List<GameObject>();
    public Transform bulletSpawnPoint;
    public Transform bulletsHolder;
    private BotAnimationController controller;

    private Transform bullet;
    private int cycle = 0;
    private float plasmaBulletSpeed = 30;

    private float cooldownTimer = 0;
    private float cooldownTime = 0.16f;

    private RaycastHit2D playerHit;
    private Vector2 playerDir;

    void Awake()
    {
        controller = transform.GetComponent<BotAnimationController>();

        foreach (Transform bullet in bulletsHolder)
            bullets.Add(bullet.gameObject);
    }

    void Update()
    {
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        //shoot at player if they are in the bot's line of sight
        playerDir = (controller.player.position - controller.shootingArm.position).normalized;
        playerHit = Physics2D.Raycast(controller.shootingArm.position, playerDir, 20f, Constants.mapOrPlayer);

        if (playerHit.collider != null && playerHit.collider.gameObject.layer == 12 && cooldownTimer <= 0)
        {
            cooldownTimer = cooldownTime;
            shootPlasmaBullet(playerDir);
        }
    }

    public void shootPlasmaBullet(Vector2 dir)
    {
        //get a bullet to the barrel of the gun
        bullet = bullets[cycle].transform;
        bullet.transform.position = bulletSpawnPoint.position;
        bullet.gameObject.SetActive(true);

        //orient it + set the bullet's velocity
        bullet.right = dir.normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * plasmaBulletSpeed;

        //cycle to a different bullet in the list next time
        cycle = ++cycle % bullets.Count;
        cooldownTimer = cooldownTime;
    }
}






