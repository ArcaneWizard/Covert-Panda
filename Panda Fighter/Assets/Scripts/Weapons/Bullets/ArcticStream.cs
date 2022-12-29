using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcticStream : Bullet
{
    private float explosionTimer = 0;

    private Rigidbody2D rig;
    private GameObject animatedExplosion;
    private Explosion explosion;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();

        animatedExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.radius = 13;
    }

    protected override void Update()
    {
        base.Update();

        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (animatedExplosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;
            animatedExplosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void startExplosion()
    {
        if (explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            animatedExplosion.SetActive(true);
            explosionTimer = 1.4f;

            StartCoroutine(explosion.damageSurroundingEntities());
        }
    }

    protected override void OnMapEnter(Transform map) => startExplosion();
    protected override void OnCreatureEnter(Transform entity) => startExplosion();
}
