using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet
{
    private SpriteRenderer rocketRenderer;
    private SpriteRenderer rocketGlareRenderer;
    private Rigidbody2D rig;
    private GameObject explosion;

    private float explosionTimer = 0;


    void Awake()
    {
        rocketRenderer = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();

        explosion = transform.GetChild(0).gameObject;
        rocketGlareRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (explosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;

            explosion.SetActive(false);
            changeRocketVisibility(true);
            gameObject.SetActive(false);
        }
    }

    public override void OnMapEnter(Transform map) => setupExplosion();
    public override void OnEntityEnter(Transform entity) => setupExplosion();

    private void setupExplosion()
    {
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        explosion.SetActive(true);
        changeRocketVisibility(false);
        explosionTimer = 1.4f;
    }

    private void changeRocketVisibility(bool visibility)
    {
        rocketRenderer.enabled = visibility;
        rocketGlareRenderer.enabled = visibility;
    }
}
