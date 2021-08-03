using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderFlare : MonoBehaviour
{
    public float lowestAlpha = 156;
    public float highestAlpha = 210;

    public Vector2 flickerSpeedRange = new Vector2(150, 200);
    private float flickerSpeed;

    private SpriteRenderer sR;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        StartCoroutine(varyFlare());
    }

    private IEnumerator varyFlare()
    {
        flickerSpeed = Random.Range(flickerSpeedRange.x, flickerSpeedRange.y);
        while (sR.color.a < highestAlpha / 255f)
        {
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, (sR.color.a + 2f / 255f));
            yield return new WaitForSeconds(1 / flickerSpeed);
        }

        flickerSpeed = Random.Range(flickerSpeedRange.x, flickerSpeedRange.y);
        while (sR.color.a > lowestAlpha / 255f)
        {
            sR.color = new Color(sR.color.r, sR.color.g, sR.color.b, (sR.color.a - 2f / 255f));
            yield return new WaitForSeconds(1 / flickerSpeed);
        }

        StartCoroutine(varyFlare());
    }
}
