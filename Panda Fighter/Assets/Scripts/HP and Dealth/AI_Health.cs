
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Health : Health
{
    protected override void Awake()
    {
        base.Awake();

        maxHP = 400;
        bulletLayer = Layer.FriendlyBullet;
        
    }
}
