using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeMec : MonoBehaviour
{
    private long num = 0;

    void Start()
    {
        Timing.RunSafeCoroutine(timer(), gameObject);
    }

    private IEnumerator<float> timer()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(0.3f);
            num += Random.Range(0, 10);
        }
    }
}
