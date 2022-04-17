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

        if (distance <= 22f)
            return damage;
        else if (distance > 22f && distance <= 121f)
            return (int)Mathf.Ceil(damage * (distance * -0.007f + 1.12f));
        else
            return 0;
    }

    protected override void OnMapEnter(Transform map) => StartCoroutine(delay());

    private IEnumerator delay() 
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }
}
