using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public override void Awake()
    {
        base.Awake();

        maxHP = 200;
        bulletLayer = Layers.enemyBullet;
        explosionLayer = Layers.enemyExplosion;
    }
}
