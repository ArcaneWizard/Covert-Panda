using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class LavaOrb : MovingBullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Explosion explosion;

    public override void OnFire(Vector2 aim)
    {
        base.OnFire(aim);
        impactExplosion.Stop();
    }

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();

        impactExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 1.3f;

        impactExplosion.gameObject.SetActive(true);
    }

    protected override void OnCreatureCollision(CollisionInfo info, Transform creature)
    {
        Timing.RunSafeCoroutine(initiateExplosion(), gameObject);
        orientExplosion(false);
    }

    protected override void OnMapCollision(CollisionInfo info)
    {
        Timing.RunSafeCoroutine(initiateExplosion(), gameObject);
        orientExplosion(true);
    }

    private void orientExplosion(bool adjustPosition)
    {
        impactExplosion.transform.localPosition = Vector3.zero;
        impactExplosion.transform.position += adjustPosition 
            ? new Vector3(-predictedNormalOfCollisionSurface.x * 1f, -predictedNormalOfCollisionSurface.y * 1f, 0f)
            : Vector3.zero;

        var main = impactExplosion.main;
       // main.startLifetimeMultiplier = (predictedNormal.y - 1) / 5f + 2.2f;
        main.gravityModifier = (predictedNormalOfCollisionSurface.y - 1) / -1.25f + 0.9f;

        var fo = impactExplosion.forceOverLifetime;
        fo.xMultiplier = (predictedNormalOfCollisionSurface.y - 1) / -0.04f + 50f;
    }

    private IEnumerator<float> initiateExplosion()
    {
        sR.enabled = false;
        rig.linearVelocity = Vector2.zero;
        impactExplosion.Play();
        Timing.RunSafeCoroutine(explosion.EnableExplosion(weaponConfiguration.Creature, weaponConfiguration.Damage), gameObject);

        yield return Timing.WaitForSeconds(impactExplosion.main.startLifetimeMultiplier + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }
}
