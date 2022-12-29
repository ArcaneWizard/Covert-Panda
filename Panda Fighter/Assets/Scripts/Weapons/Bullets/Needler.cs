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
        explosion.radius = 1.1f;
    }

    public override void Reset()
    {
        base.Reset();
        transform.GetComponent<ParticleSystem>().Stop();
    }

    protected override void OnMapEnter(Transform map) => StartCoroutine(initiateExplosion());
    protected override void OnCreatureEnter(Transform entity) => StartCoroutine(initiateExplosion());

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
