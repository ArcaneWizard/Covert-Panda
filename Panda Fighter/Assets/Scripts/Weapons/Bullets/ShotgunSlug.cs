using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShotgunSlug : Bullet
{
    private Vector3 spawnPosition;
    private float trailOffset;
    private TrailRenderer trailRenderer;

    public override void ConfigureBulletBeforeFiring(Vector2 aim, bool doesBulletHaveArcMotion, bool doesBulletStickToCreatures)
    {
        base.ConfigureBulletBeforeFiring(aim, doesBulletHaveArcMotion, doesBulletStickToCreatures);
        spawnPosition = transform.position;

        if (trailRenderer == null)
            trailRenderer = transform.GetComponent<TrailRenderer>();
        
        trailRenderer.sortingLayerName = "invisible";
        trailRenderer.time = UnityEngine.Random.Range(0.05f, 0.08f);
        trailOffset = UnityEngine.Random.Range(0.8f, 4); 
    }

    protected override void Update() 
    {
        base.Update();

        if (squaredDistance(transform.position, spawnPosition) > trailOffset && gameObject.activeSelf) 
        {
           StartCoroutine(delayBulletStreaks());
           trailOffset = Int32.MaxValue;
        }
    }

    protected override int damage()
    {
        float distance = squaredDistance(transform.position, spawnPosition);

        if (distance <= 200f)
            return base.damage();
        else if (distance > 200f && distance <= 400f)
            return (int)Mathf.Ceil(base.damage() * (distance * -0.005f + 2f));
        else
            return 0;
    }

    protected override void OnMapEnter(Transform map) => StartCoroutine(madeContact());
    protected override void OnCreatureEnter(Transform creature) => StartCoroutine(madeContact());

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
