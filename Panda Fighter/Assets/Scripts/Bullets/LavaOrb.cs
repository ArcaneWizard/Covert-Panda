using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaOrb : Bullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Rigidbody2D rig;

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

    private void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        impactExplosion = transform.GetChild(0).GetComponent<ParticleSystem>();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    private void OnEnable() => impactExplosion.Stop();
    public override void OnEntityEnter(Transform entity) => StartCoroutine(initiateExplosion());
    public override void OnMapEnter(Transform entity) => StartCoroutine(initiateExplosion());

    private IEnumerator initiateExplosion()
    {
        sR.enabled = false;
        rig.velocity = Vector2.zero;
        impactExplosion.Play();

        yield return new WaitForSeconds(impactExplosion.main.startLifetime.constant + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
