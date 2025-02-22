using UnityEngine;

// Manages and implements visual effects on the creature's limbs

public class LimbSprite : MonoBehaviour
{
    public CentralAbilityHandler CentralAbilityHandler;

    private SpriteRenderer sR;
    private Color color;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        color = sR.color;
    }

    void Update()
    {
        sR.color = (CentralAbilityHandler?.IsInvisible == true)
            ? new Color(color.r, color.g, color.b, 0.4f)
            : new Color(color.r, color.g, color.b, 1f);
    }

}
