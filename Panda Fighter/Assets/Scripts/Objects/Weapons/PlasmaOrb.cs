using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaOrb : MonoBehaviour
{
    public float delayTillExplosion;
    private float explosionTimer = 0;

    private SpriteRenderer sR;
    private Rigidbody2D rig;

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

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
        yield return new WaitForSeconds(delayTillExplosion);

        sR.enabled = false;
        transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.GetChild(0).gameObject.SetActive(true);
        rig.constraints = RigidbodyConstraints2D.FreezeAll;

        explosionTimer = 1.4f;
    }
}
