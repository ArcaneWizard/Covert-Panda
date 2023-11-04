using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTEst : MonoBehaviour
{
    private float timer = 0.3f;
    private long a = 0;

    void Update()
    {
        if (timer > 0f)
            timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            a += Random.Range(0, 10);
            timer = 0.3f;
        }
    }
}
