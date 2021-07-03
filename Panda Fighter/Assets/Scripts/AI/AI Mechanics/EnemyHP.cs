using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public int hp;
    private int maxHP = 100;

    public Transform healthBar;
    private float healthBarSize;

    private Transform alien;
    private Vector3 barOffset;

    void Awake()
    {
        hp = maxHP;
        healthBarSize = healthBar.localScale.x;

        alien = transform.GetChild(0).transform;
        barOffset = healthBar.parent.position - alien.position;
    }

    void Update()
    {
        if (hp >= 0)
            healthBar.localScale = new Vector3(healthBarSize * (float)hp / (float)maxHP, healthBar.localScale.y, healthBar.localScale.z);
        else
            healthBar.localScale = new Vector3(0f, healthBar.localScale.y, healthBar.localScale.z);

        healthBar.parent.position = alien.position + barOffset;
    }
}
