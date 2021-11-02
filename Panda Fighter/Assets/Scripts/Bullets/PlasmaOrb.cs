using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaOrb : MonoBehaviour
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;

    [Range(0, 3)]
    private float delayTillExplosion = 0.5f;

    private SpriteRenderer sR;
    private Rigidbody2D rig;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (!sR.enabled)
        {
            sR.enabled = true;
            transform.GetChild(0).gameObject.SetActive(false);
            rig.constraints = RigidbodyConstraints2D.None;
            gameObject.SetActive(false);
        }
    }

    public IEnumerator startTimedPlasmaExplosion()
    {
        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(delayTillExplosion);

        sR.enabled = false;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.GetChild(0).gameObject.SetActive(true);
        explosionTimer = 1.4f;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11 && explosionTimer <= 0f && rig.constraints != RigidbodyConstraints2D.FreezeAll)
            StartCoroutine(startTimedPlasmaExplosion());

    }
}
