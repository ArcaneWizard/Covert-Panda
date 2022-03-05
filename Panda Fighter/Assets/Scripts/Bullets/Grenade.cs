using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Bullet
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;
    private float timeTillExplosion = 1.2f;

    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private GameObject animatedExplosion;
    private Explosion explosion;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
        animatedExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();

        explosion.radius = 12;
    }

    public void startExplosionTimer() => StartCoroutine(eStartExplosionTimer());

    private IEnumerator eStartExplosionTimer()
    {
        yield return new WaitForSeconds(timeTillExplosion);

        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        sR.enabled = false;
        animatedExplosion.SetActive(true);
        explosionTimer = 1.4f;

        StartCoroutine(explosion.damageSurroundingEntities());
    }

    public override void OnMapEnter(Transform map) {}

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (animatedExplosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;

            sR.enabled = true;
            animatedExplosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
