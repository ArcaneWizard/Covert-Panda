using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShotgunSlug : Bullet
{
    private Vector3 spawnPosition;
    private float trailOffset;
    private TrailRenderer trailRenderer;

    private void OnEnable() 
    {
        spawnPosition = transform.position;

        if (trailRenderer == null)
            trailRenderer = transform.GetComponent<TrailRenderer>();
        
        trailRenderer.sortingLayerName = "invisible";
        trailRenderer.time = UnityEngine.Random.Range(0.05f, 0.08f);
        trailOffset = UnityEngine.Random.Range(0.8f, 4); 
    }

    public override void Update() 
    {
        base.Update();

        if (sqrDistance(transform.position, spawnPosition) > trailOffset && gameObject.activeSelf) 
        {
           StartCoroutine(delayBulletStreaks());
           trailOffset = Int32.MaxValue;
        }
    }

    public override int Damage()
    {
        int damage = transform.parent.GetComponent<WeaponConfiguration>().BulletDmg;
        float distance = sqrDistance(transform.position, spawnPosition);

        if (distance <= 200f)
            return damage;
        else if (distance > 200f && distance <= 400f)
            return (int)Mathf.Ceil(damage * (distance * -0.005f + 2f));
        else
            return 0;
    }

    protected override void OnMapEnter(Transform map) => StartCoroutine(madeContact());
    public override void OnCreatureEnter(Transform creature) => StartCoroutine(madeContact());

    private IEnumerator madeContact() 
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        gameObject.SetActive(false);
    }

    private IEnumerator delayBulletStreaks() 
    {
        trailRenderer.sortingLayerName = "effects";
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.055f, 0.06f));
        trailRenderer.sortingLayerName = "invisible";
    }
}
