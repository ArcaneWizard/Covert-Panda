using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    protected override void Awake()
    {
        base.Awake();

        maxHP = 600;
        bulletLayer = Layer.EnemyBullet;
    }

}
