using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Bullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Explosion explosion;

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();

        impactExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 1.1f;
    }

    public override void StartCollisionDetection(Vector2 aim, BulletMovementAfterFiring movementAfterFiring, bool doesBulletStickToCreatures)
    {
        base.StartCollisionDetection(aim, movementAfterFiring, doesBulletStickToCreatures);
        impactExplosion.GetComponent<ParticleSystem>().Stop();
    }

    protected override void onMapEnter(Transform map) => StartCoroutine(initiateExplosion());
    protected override void onCreatureEnter(Transform entity) => StartCoroutine(initiateExplosion());

    private IEnumerator initiateExplosion()
    {
        sR.enabled = false;
        rig.velocity = Vector2.zero;
        impactExplosion.Play();
        StartCoroutine(explosion.damageSurroundingEntities());

        yield return new WaitForSeconds(impactExplosion.main.startLifetime.constant + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
