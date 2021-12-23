using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibilityEffect : MonoBehaviour
{
    public CentralAbilityHandler centralAbilityHandler;
    private SpriteRenderer sR;

    private Color color;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        color = sR.color;
    }

    void Update()
    {
        sR.color = (centralAbilityHandler.isInvisible)
            ? new Color(color.r, color.g, color.b, 0.3f)
            : new Color(color.r, color.g, color.g, 1f);
    }
}
