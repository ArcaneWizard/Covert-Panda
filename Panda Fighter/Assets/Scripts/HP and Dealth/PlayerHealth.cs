using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public override void Awake()
    {
        base.Awake();

        maxHP = 200;
        currentHP = maxHP;
        bulletLayer = Layers.enemyBullet;
        explosionLayer = Layers.enemyExplosion;
    }

    void Update()
    {
        DebugGUI.debugText6 = currentHP.ToString() + "/" + maxHP.ToString();
    }
}
