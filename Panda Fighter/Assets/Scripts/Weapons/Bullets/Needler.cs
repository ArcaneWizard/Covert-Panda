using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needler : Bullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private Explosion explosion;

    private void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();

        impactExplosion = transform.GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 1.1f;
    }

    public override void OnFire(Vector2 aim, BulletMovementAfterFiring movementAfterFiring, bool doesBulletStickToCreatures)
    {
        base.OnFire(aim, movementAfterFiring, doesBulletStickToCreatures);
        transform.GetComponent<ParticleSystem>().Stop();
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
