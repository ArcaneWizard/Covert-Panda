using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaOrb : Bullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private Explosion explosion;

    private void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();

        impactExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        explosion = transform.GetComponent<Explosion>();
        explosion.radius = 1.3f;

        impactExplosion.gameObject.SetActive(true);
    }

    public override void ConfigureBulletBeforeFiring(Vector2 aim, bool doesBulletHaveArcMotion, bool doesBulletStickToCreatures)
    {
        base.ConfigureBulletBeforeFiring(aim, doesBulletHaveArcMotion, doesBulletStickToCreatures);
        impactExplosion.Stop();
    }

    private void OrientExplosion(bool adjustPosition)
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

    protected override void OnCreatureEnter(Transform entity) 
    {
         StartCoroutine(initiateExplosion());
         OrientExplosion(false);
    }
    protected override void OnMapEnter(Transform map) 
    {
        StartCoroutine(initiateExplosion());
        OrientExplosion(true);
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
