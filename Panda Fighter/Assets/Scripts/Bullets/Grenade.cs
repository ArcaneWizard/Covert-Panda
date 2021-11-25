using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;
    private float timeTillExplosion = 1.2f;

    private SpriteRenderer sR;
    private Rigidbody2D rig;
    private GameObject explosion;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
        explosion = transform.GetChild(0).gameObject;
    }

    public void startExplosionTimer() => StartCoroutine(eStartExplosionTimer());

    private IEnumerator eStartExplosionTimer()
    {
        yield return new WaitForSeconds(timeTillExplosion);

        rig.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.localEulerAngles = new Vector3(0, 0, 0);

        sR.enabled = false;
        explosion.SetActive(true);
        explosionTimer = 1.4f;
    }

    void Update()
    {
        if (explosionTimer > 0f)
            explosionTimer -= Time.deltaTime;

        else if (explosion.activeSelf)
        {
            rig.constraints = RigidbodyConstraints2D.None;

            sR.enabled = true;
            explosion.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
