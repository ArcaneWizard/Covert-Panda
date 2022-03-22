
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Health : Health
{
    public override void Awake()
    {
        base.Awake();

        maxHP = 300;
        bulletLayer = Layers.friendlyBullet;
    }
}
