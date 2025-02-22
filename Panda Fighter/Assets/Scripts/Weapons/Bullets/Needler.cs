using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Needler : MovingBullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Explosion explosion;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        transform.GetComponent<ParticleSystem>().Stop();
    }

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();

        impactExplosion = transform.GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 1.1f;
    }

    protected override void OnMapCollision(CollisionInfo info) => Timing.RunSafeCoroutine(initiateExplosion(), gameObject);
    protected override void OnCreatureCollision(CollisionInfo info, Transform creature) => Timing.RunSafeCoroutine(initiateExplosion(), gameObject);

    private IEnumerator<float> initiateExplosion()
    {
        sR.enabled = false;
        rig.linearVelocity = Vector2.zero;
        impactExplosion.Play();
        Timing.RunSafeCoroutine(explosion.EnableExplosion(weaponConfiguration.Creature, weaponConfiguration.Damage), gameObject);

        yield return Timing.WaitForSeconds(impactExplosion.main.startLifetime.constant + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
