using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlasmaOrb : MonoBehaviour
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;

    private LayerMask surfaces = 1 << 11 | 1 << 14;

    [Range(0, 3)]
    private float delayTillExplosion = 0.5f;

    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private GameObject explosion;

    private Vector3 contactLocation, surfaceContactLocation;
    private Transform trackingSurface;


    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
        explosion = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (!sR.enabled)
        {
            rig.constraints = RigidbodyConstraints2D.None;
            sR.enabled = true;
            explosion.SetActive(false);
            trackingSurface = null;

            gameObject.SetActive(false);
        }

        if (trackingSurface != null)
            transform.position = trackingSurface.position - surfaceContactLocation + contactLocation;
    }

    public IEnumerator startTimedPlasmaExplosion(Transform surface)
    {
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        contactLocation = transform.position;
        trackingSurface = surface;
        surfaceContactLocation = new Vector3(trackingSurface.position.x, trackingSurface.position.y, 0);

        yield return new WaitForSeconds(delayTillExplosion);

        transform.localEulerAngles = new Vector3(0, 0, 0);
        explosion.SetActive(true);
        explosionTimer = 1.4f;

        yield return new WaitForSeconds(0.24f);
        sR.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.layer == 11 || col.gameObject.layer == 14) && explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll)
            StartCoroutine(startTimedPlasmaExplosion(col.transform));
    }
}
