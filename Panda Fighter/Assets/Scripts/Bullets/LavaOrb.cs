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
    }

    private void OnEnable() => impactExplosion.Stop();

    public void OrientExplosion(Vector2 normal)
    {
        impactExplosion.transform.position = transform.position +
            new Vector3(-normal.x * 1f, -normal.y * 1f, 0f);

        var main = impactExplosion.main;
        main.startLifetimeMultiplier = (normal.y - 1) / 5f + 2.2f;
        main.gravityModifier = (normal.y - 1) / -1.25f + 0.9f;

        var fo = impactExplosion.forceOverLifetime;
        fo.xMultiplier = (normal.y - 1) / -0.04f + 50f;
    }

    public override void OnEntityEnter(Transform entity) => StartCoroutine(initiateExplosion());
    public override void OnMapEnter(Transform entity) => StartCoroutine(initiateExplosion());

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
