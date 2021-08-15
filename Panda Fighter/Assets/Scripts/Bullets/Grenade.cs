using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    private int surfacesTouched = 0;
    private float explosionTimer = 0;

    private SpriteRenderer sR;
    private Rigidbody2D rig;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        rig = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (surfacesTouched >= 3)
        {
            sR.enabled = false;
            transform.localEulerAngles = new Vector3(0, 0, 0);
            transform.GetChild(0).gameObject.SetActive(true);
            rig.constraints = RigidbodyConstraints2D.FreezeAll;

            explosionTimer = 1.4f;
            surfacesTouched = 0;
        }

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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 11 && explosionTimer <= 0f)
            surfacesTouched++;
    }
}
