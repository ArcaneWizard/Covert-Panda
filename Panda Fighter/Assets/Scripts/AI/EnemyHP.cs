﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    private int hp;
    private int maxHP = 100;

    public Transform healthBar;
    private float healthBarSize;

    void Start()
    {
        hp = maxHP;
        healthBarSize = healthBar.localScale.x;
    }

    void Update()
    {
        healthBar.localScale = new Vector3(healthBarSize * (float)hp / (float)maxHP, healthBar.localScale.y, healthBar.localScale.z);

        if (hp <= 0)
            gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Boomerang" || col.gameObject.tag == "Grenade")
        {
            hp -= 20;
            col.gameObject.SetActive(false);
        }
    }
}