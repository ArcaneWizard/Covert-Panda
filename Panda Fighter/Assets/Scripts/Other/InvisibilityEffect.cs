using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InvisibilityEffect : MonoBehaviour
{
    public CentralAbilityHandler centralAbilityHandler;
    private SpriteRenderer sR;

    private Color color;

    private Transform bob;
    private float counter;

    void Awake()
    {
        sR = transform.GetComponent<SpriteRenderer>();
        color = sR.color;
    }

    /*private void OnValidate()
    {
        counter = 0;
        bob = transform;
        while (!centralAbilityHandler && counter < 7f)
        {
            counter++;

            bob = bob.parent;

            if (bob.transform.GetComponent<CentralAbilityHandler>())
                centralAbilityHandler = bob.transform.GetComponent<CentralAbilityHandler>();
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }*/

    void Update()
    {
        sR.color = (centralAbilityHandler.isInvisible)
            ? new Color(color.r, color.g, color.b, 0.4f)
            : new Color(color.r, color.g, color.g, 1f);
    }
}
