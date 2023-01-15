using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [SerializeField]
    private Texture[] textures;

    [SerializeField]
    private float inverseFPS = 1 / 2;

    private int animationStep;
    private float fpsCounter;

    private void Awake() => lineRenderer = transform.GetComponent<LineRenderer>();

    void Update()
    {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= inverseFPS)
        {
            animationStep = ++animationStep % textures.Length;
            lineRenderer.material.SetTexture("_MainTex", textures[animationStep]);
            fpsCounter = 0f;
        }
    }
}
