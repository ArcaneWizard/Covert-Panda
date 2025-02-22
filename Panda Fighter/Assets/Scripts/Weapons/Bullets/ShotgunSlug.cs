using System;
using System.Collections;

using UnityEngine;

public class ShotgunSlug : MovingBullet
{
    private Vector3 spawnPosition;
    private float trailOffset;
    private TrailRenderer trailRenderer;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        spawnPosition = transform.position;

        if (trailRenderer == null)
            trailRenderer = transform.GetComponent<TrailRenderer>();

        trailRenderer.sortingLayerName = "invisible";
        trailRenderer.time = UnityEngine.Random.Range(0.05f, 0.08f);
        trailOffset = UnityEngine.Random.Range(0.8f, 4);
    }

    void Update()
    {
        if (MathX.GetSquaredDistance(transform.position, spawnPosition) > trailOffset && gameObject.activeSelf) {
            StartCoroutine(delayBulletStreaks());
            trailOffset = Int32.MaxValue;
        }
    }

    protected override float DamageMultiplier()
    {
        float distance = MathX.GetSquaredDistance(transform.position, spawnPosition);

        if (distance <= 200f)
            return 1f;
        else if (distance > 200f && distance <= 400f)
            return (float)(distance * -0.005f + 2f);
        else
            return 0f;
    }

    protected override void OnMapCollision(CollisionInfo info) => StartCoroutine(madeContact());
    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) => StartCoroutine(madeContact());

    private IEnumerator madeContact()
    {
        rig.linearVelocity = Vector2.zero;
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
