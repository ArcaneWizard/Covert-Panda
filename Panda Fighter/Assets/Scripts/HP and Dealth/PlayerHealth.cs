using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public RespawningText respawnText;
    public GameObject inventory;

    protected override void Awake()
    {
        base.Awake();

        maxHP = 600;
        bulletLayer = Layer.EnemyBullet;
    }

    protected override void UponDying() 
    {
        inventory.SetActive(false);
        respawnText.StartRespawnCountdown(respawnTime);
        base.UponDying();
    }

    protected override void BeforeRespawning() 
    {
        inventory.SetActive(true);
        base.BeforeRespawning();
    }
}
