using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : MovingBullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Explosion explosion;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        impactExplosion.GetComponent<ParticleSystem>().Stop();
    }

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();

        impactExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 1.1f;
    }
    protected override void OnMapCollision(CollisionInfo info) => StartCoroutine(initiateExplosion());
    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) => StartCoroutine(initiateExplosion());

    private IEnumerator initiateExplosion()
    {
        sR.enabled = false;
        rig.velocity = Vector2.zero;
        impactExplosion.Play();
        Timing.RunSafeCoroutine(explosion.EnableExplosion(), gameObject);

        yield return new WaitForSeconds(impactExplosion.main.startLifetime.constant + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
