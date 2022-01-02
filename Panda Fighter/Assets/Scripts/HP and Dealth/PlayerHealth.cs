using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    public Image hpBar;
    private Vector2 positionOffset;
    private Vector2 playerPosition;

    public override void Awake()
    {
        base.Awake();

        maxHP = 400;
        currentHP = maxHP;
        bulletLayer = Layers.enemyBullet;
        explosionLayer = Layers.enemyExplosion;
    }

    public override void Update() 
    {
        base.Update();
        hpBar.fillAmount = (float) currentHP / (float) maxHP;
        hpBar.transform.parent.GetComponent<RectTransform>().position = positionOffset + new Vector2(transform.position.x, transform.position.y);
    }
}
