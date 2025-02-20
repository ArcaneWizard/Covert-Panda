/*using System;
using System.Security.Cryptography;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.InputManagerEntry;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using MEC;
using System;

public class Test2 : MonoBehaviour
{
    private float villagers2 = 0.1f;
    private float time;
    private float timer = 0f;
    private float index = 0f;

    float distance;
    float doubleDistance;

    public List<ExecutionDelay> f;
    public List<Action> a;

    float r;

    private void Start()
    {
        Timing.RunCoroutine(alpha());
    }

    *//*private IEnumerator testVillage()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
            StartCoroutine(testVillage2());
        }
    }
    *//*
    private IEnumerator<float> alpha()
    {
        r = (transform.GetSiblingIndex() % 100) / 100f;
        yield return Timing.WaitForSeconds(r);
        Timing.RunCoroutine(testVillage());
    }


    private IEnumerator<float> beta()
    {
        r = (transform.GetSiblingIndex() % 100) / 100f;
        yield return Timing.WaitForOneFrame;

    }


    private IEnumerator<float> testVillage()
    {
        int i = 0;
        while (i < 220) {
            yield return 0;
            i++;
         }

        yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
        i++;
        yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
        i++;
        yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
        i++;
        yield return Timing.WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
        i++;

//        Timing.K
    }
}*/