using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : MonoBehaviour
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

    private void OnEnable()
    {
        transform.GetComponent<ParticleSystem>().Stop();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 11)
            StartCoroutine(initiateExplosion());
    }

    private IEnumerator initiateExplosion()
    {
        sR.enabled = false;
        rig.velocity = Vector2.zero;
        impactExplosion.Play();

        yield return new WaitForSeconds(impactExplosion.main.duration + 0.1f);

        sR.enabled = true;
        gameObject.SetActive(false);
    }

}
