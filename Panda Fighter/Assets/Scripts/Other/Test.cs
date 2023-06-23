using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.InputManagerEntry;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    private float villagers2 = 0.1f;
    private float time;
    private float timer = 0f;
    private float index = 0f;

    float distance;
    float doubleDistance;

    private AdvCoroutine advCoroutine;

    // public static List<float> f = new List<float>();
    // public static List<Action> a = new List<Action>();


    public List<ExecutionDelay> f;
    public List<Action> a;

    private TimedCode fastCouroutine;
    /*void Start()
    {
        f = new List<ExecutionDelay>();
        a = new List<Action>();
       
         fastCouroutine = new TimedCode(f, a);
        f.Clear();
        for (int i = 0; i < 6; i++)
            f.Add(new ExecutionDelay(UnityEngine.Random.Range(0.18f, 0.2f)));

        a.Clear();
        for (int i = 0; i < 1; i++)
        {
            a.Add(bob);
            a.Add(joe);
            a.Add(cow);
            a.Add(dee);
            a.Add(dee);
            a.Add(nah);
        }
        void bob()
            {
              

                distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
            }

            void joe()
            {
                doubleDistance = 2 * distance;
            }

            void cow()
            {
                doubleDistance = 3 * distance;
            }

            void dee()
            {
                doubleDistance = 4 * distance;
            }

            void nah()
            {
                doubleDistance = 5 * distance;
             time = Time.time;
            f.Clear();
            for (int i = 0; i < 6; i++)
                f.Add(new ExecutionDelay(UnityEngine.Random.Range(0.18f, 0.2f)));

            a.Clear();
            for (int i = 0; i < 1; i++)
            {
                a.Add(bob);
                a.Add(joe);
                a.Add(cow);
                a.Add(dee);
                a.Add(nah);
                a.Add(nah);
            }

            fastCouroutine.Start();
        }
        

        //StartCoroutine(testVillage());
        fastCouroutine.Start();
    }*/

    private void Start()
    {
       // StartCoroutine(testVillage());
    }

    /*private void FixedUpdate()
    {
        RaycastHit2D a = Physics2D.Raycast(transform.position, Vector2.down, 20f, LayerMasks.map);
        RaycastHit2D b = Physics2D.Raycast(transform.position, Vector2.down, 20f, LayerMasks.map);
        RaycastHit2D c = Physics2D.Raycast(transform.position, Vector2.down, 20f, LayerMasks.map);
        RaycastHit2D d = Physics2D.Raycast(transform.position, Vector2.down, 20f, LayerMasks.map);
    }*/

    private int i = 0;
    //  void Update()
    //{
    //  fastCouroutine?.Update();
    //}

    /*private void Update()
    {
        fastCouroutine.Update();

        /*if (fastCouroutine.doStuff)
        {

        }

        if (timer < 0.55f)
            timer += Time.deltaTime;
        else
            timer = 0f;

        if (timer > 0.1f && index == 0)
        {
            index++;
            distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
        }
        else if (timer > 0.2f && index == 1)
        {
            index++;
            doubleDistance = 2 * distance;
        }
        else if (timer > 0.3f && index == 2)
        {
            index++;
            doubleDistance = 3 * distance;
        }
        else if (timer > 0.4f && index == 3)
        {
            index++;
            doubleDistance = 4 * distance;
        }
        else if (timer > 0.5f && index == 4)
        {
            index = 0;
            doubleDistance = 5 * distance;
        }*/

    /*private void Update()
    {
        if (villagers2 > 0f)
            villagers2 -= Time.deltaTime;

        else if (villagers2 <= 0f)
        {
            villagers2 = UnityEngine.Random.Range(0.05f, 0.1f);
            a += 1;
        }
    }*/


    /*private IEnumerator testVillage()
    {
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.22f));
            StartCoroutine(testVillage2());
        }
    }
    */

    private void FixedUpdate()
    {
        
    }

    private IEnumerator testVillage()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.18f, 0.25f));
        distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));

        StartCoroutine(testVillage());
    }

    private IEnumerator testVillage2()
    {
        distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
        yield return null;
    }
}