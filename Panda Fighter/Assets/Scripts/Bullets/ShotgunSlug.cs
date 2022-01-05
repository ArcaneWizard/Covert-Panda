using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunSlug : Bullet
{
    private Vector3 spawnPosition;

    private void OnEnable() => spawnPosition = transform.position;

    public override int Damage()
    {
        int damage = transform.parent.GetComponent<WeaponConfiguration>().bulletDmg;
        Vector3 distanceVector = transform.position - spawnPosition;
        float distance = (distanceVector.x * distanceVector.x) + (distanceVector.y * distanceVector.y);

        if (distance <= 6.2f)
            return damage;
        else if (distance > 6.2f && distance <= 10f)
            return (int)Mathf.Ceil(damage * 1f / (distance - 5f));
        else
            return 0;
    }
}
