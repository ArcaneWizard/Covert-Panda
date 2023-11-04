using MEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Mec : MonoBehaviour
{
    long num = 0;
    void Start()
    {
       // StartCoroutine(timer2());
        Timing.RunSafeCoroutine(timer(), gameObject);
    }

    private IEnumerator<float> timer()
    {
            yield return Timing.WaitForSeconds(0.3f);
            num += Random.Range(0, 10); 
        Timing.RunCoroutine(timer());
    }

    private IEnumerator timer2()
    {
            yield return new WaitForSeconds(0.3f);
            num += Random.Range(0, 10);
            StartCoroutine(timer2());
    }
}
