using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet
{
    private SpriteRenderer rocketRenderer;
    private SpriteRenderer rocketGlareRenderer;
    private Rigidbody2D rig;
    private GameObject physicalExplosion;
    private Explosion explosion;

    private float explosionTimer = 0;


    void Awake()
    {
        rocketRenderer = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
        rocketGlareRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();

        physicalExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 7f;
    }

    protected override void Update()
    {
        base.Update();
        
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (physicalExplosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;

            physicalExplosion.SetActive(false);
            changeRocketVisibility(true);
            gameObject.SetActive(false);
        }
    }

    protected override void onMapEnter(Transform map) => setupExplosion();
    protected override void onCreatureEnter(Transform entity) => setupExplosion();

    private void setupExplosion()
    {
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        physicalExplosion.SetActive(true);
        changeRocketVisibility(false);
        explosionTimer = 1.4f;

        StartCoroutine(explosion.damageSurroundingEntities());
    }

    private void changeRocketVisibility(bool visibility)
    {
        rocketRenderer.enabled = visibility;
        rocketGlareRenderer.enabled = visibility;
    }
}
