using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlasmaOrb : Bullet
{
    private float explosionTimer = 0;
    private LayerMask surfaces = 1 << 11 | 1 << 14;

    private float delayTillExplosion = 0.9f;

    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private GameObject physicalExplosion;
    private Explosion explosion;

    private Vector3 contactLocation, surfaceContactLocation;
    private Transform trackingSurface;
    private Transform creature;


    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();

        physicalExplosion = transform.GetChild(0).gameObject;
        explosion = transform.GetComponent<Explosion>();
        explosion.radius = 12f;
    }

    void OnEnable() 
    {
        explosionTimer = 0f;
        rig.constraints = RigidbodyConstraints2D.None;
    }

    void Update()
    {
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
        if (trackingSurface != null && creature != null && !creature.GetComponent<Health>().isDead)
            transform.position = trackingSurface.position - surfaceContactLocation + contactLocation;
        
      //  Debug.LogFormat("{0}, {1}. {2}", trackingSurface, creature, !creature.GetComponent<Health>().isDead);
    }

    public IEnumerator startTimedPlasmaExplosion(Transform surface)
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

    protected override void OnMapEnter(Transform map) 
    {
         StartCoroutine(startTimedPlasmaExplosion(map));
         creature = null;
    }
    
    public override void OnCreatureEnter(Transform entity) 
    {
        StartCoroutine(startTimedPlasmaExplosion(whatItStuckTo));
        creature = entity;
    }
}
