using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaOrb : Bullet
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
        explosion.Radius = 1.3f;

        impactExplosion.gameObject.SetActive(true);
    }

    protected override void onFire()
    {
        impactExplosion.Stop();
    }

    private void orientExplosion(bool adjustPosition)
    {
        impactExplosion.transform.localPosition = Vector3.zero;
        impactExplosion.transform.position += adjustPosition 
            ? new Vector3(-predictedNormal.x * 1f, -predictedNormal.y * 1f, 0f)
            : Vector3.zero;

        var main = impactExplosion.main;
       // main.startLifetimeMultiplier = (predictedNormal.y - 1) / 5f + 2.2f;
        main.gravityModifier = (predictedNormal.y - 1) / -1.25f + 0.9f;

        var fo = impactExplosion.forceOverLifetime;
        fo.xMultiplier = (predictedNormal.y - 1) / -0.04f + 50f;
    }

    protected override void onCreatureEnter(Transform entity) 
    {
         StartCoroutine(initiateExplosion());
         orientExplosion(false);
    }
    protected override void onMapEnter(Transform map) 
    {
        StartCoroutine(initiateExplosion());
        orientExplosion(true);
    }

    private IEnumerator initiateExplosion()
    {
        sR.enabled = false;
        rig.velocity = Vector2.zero;
        impactExplosion.Play();
        StartCoroutine(explosion.damageSurroundingEntities());

        yield return new WaitForSeconds(impactExplosion.main.startLifetimeMultiplier + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
