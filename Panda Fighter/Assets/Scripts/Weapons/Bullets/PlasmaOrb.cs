using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlasmaOrb : Bullet
{
    private float explosionTimer = 0;

    private float delayTillExplosion = 0.9f;

    private SpriteRenderer sR;
    private GameObject physicalExplosion;
    private Explosion explosion;

    private Vector3 contactLocation, surfaceContactLocation;
    private Transform trackingSurface;
    private Transform creature;

    protected override void Awake()
    {
        base.Awake();
        sR = transform.GetComponent<SpriteRenderer>();

        physicalExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.Radius = 12f;
    }

    public override void StartCollisionDetection(Vector2 aim, BulletMovementAfterFiring movementAfterFiring, bool doesBulletStickToCreatures)
    {
        base.StartCollisionDetection(aim, movementAfterFiring, doesBulletStickToCreatures);
        explosionTimer = 0f;
        rig.constraints = RigidbodyConstraints2D.None;
    }

    protected override void Update()
    {
        base.Update();
        
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (!sR.enabled)
        {
            rig.constraints = RigidbodyConstraints2D.None;
            sR.enabled = true;
            physicalExplosion.SetActive(false);
            trackingSurface = null;

            gameObject.SetActive(false);
        }

        // update the sticky orb's position if it's clinging to a creature (that's alive)
        if (trackingSurface != null && creature != null && !creature.GetComponent<Health>().IsDead)
            transform.position = trackingSurface.position - surfaceContactLocation + contactLocation;
    }

    private IEnumerator startTimedPlasmaExplosion(Transform surface)
    {
        if (explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll)
        {
            rig.constraints = RigidbodyConstraints2D.FreezeAll;
            contactLocation = transform.position;
            trackingSurface = surface;
            surfaceContactLocation = new Vector3(trackingSurface.position.x, trackingSurface.position.y, 0);

            yield return new WaitForSeconds(delayTillExplosion);

            transform.localEulerAngles = new Vector3(0, 0, 0);
            physicalExplosion.SetActive(true);
            explosionTimer = 1.4f;
            sR.enabled = false;

            StartCoroutine(explosion.damageSurroundingEntities());
        }
    }

    protected override void onMapEnter(Transform map) 
    {
         StartCoroutine(startTimedPlasmaExplosion(map));
         creature = null;
    }
    
    protected override void onCreatureEnter(Transform entity) 
    {
        StartCoroutine(startTimedPlasmaExplosion(entity));
        creature = entity;
    }
}
