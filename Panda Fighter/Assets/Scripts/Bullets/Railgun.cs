using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : Bullet
{
    private ParticleSystem impactExplosion;
    private SpriteRenderer sR;
    private Rigidbody2D rig;

    private void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        impactExplosion = transform.GetComponent<ParticleSystem>();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    private void OnEnable() => transform.GetComponent<ParticleSystem>().Stop();

    public override void OnMapEnter(Transform map) => StartCoroutine(initiateExplosion());
    public override void OnEntityEnter(Transform entity) => StartCoroutine(initiateExplosion());

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
