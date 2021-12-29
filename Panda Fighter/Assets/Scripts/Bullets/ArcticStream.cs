using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ArcticStream : Bullet
{
    private float explosionTimer = 0;

    private Rigidbody2D rig;
    private GameObject explosion;

    void Awake()
    {
        rig = transform.GetComponent<Rigidbody2D>();
        explosion = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (explosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;
            explosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    private void startExplosion()
    {
        if (explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            explosion.SetActive(true);
            explosionTimer = 1.4f;
        }
    }

    public override void OnMapEnter(Transform map) => startExplosion();
    public override void OnEntityEnter(Transform entity) => startExplosion();
}
