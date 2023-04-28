using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.InputManagerEntry;
using static UnityEngine.Rendering.DebugUI.Table;
using UnityEngine.UIElements;

public class Test : MonoBehaviour
{
    private float villagers2 = 0.1f;
    private float time;
    private float timer = 0f;
    private float index = 0f;

    private AdvCoroutine advCoroutine;

    // public static List<float> f = new List<float>();
    // public static List<Action> a = new List<Action>();


    public List<ExecutionDelay> f;
    public List<Action> a;

    private ActionFlowOverTime fastCouroutine;
    private void Start()
    {
       /* if (f == null)
        {
            f = new List<ExecutionDelay>();
            for (int i = 0; i < 3; i++)
                f.Add(ExecutionDelay.Waiting);
        }

        if (a == null)
        {
            a = new List<Action>();
            for (int i = 0; i < 1; i++)
            {
                a.Add(bob);
                a.Add(joe);
                a.Add(cow);
            }
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
        }
       */
          // fastCouroutine = new ActionFlowOverTime(f, a);
     
         StartCoroutine(testVillage());
        //fastCouroutine.Start(true);
    }

    private bool yay;
    private void finale()
    {
        yay = true;
    }

    private int i = 0;
   /* private void Update()
    {
        fastCouroutine.Update();

        f[i].StopWaiting();
        i = (i + 1) % f.Count;
    }*/

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

    private IEnumerator testVillage()
    {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.35f));
            distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.35f));
            doubleDistance = 4 * 2;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.35f));
            doubleDistance = 4 * distance;
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.25f, 0.35f));
        doubleDistance = 4 * 3;

        int i = 100;
        while (i > 0f) {
            yield return null;
            i--;
         }

        StartCoroutine(testVillage());
    }

    private IEnumerator testVillage2()
    {
            distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
            yield return null;
    }
    

    float distance;
    float doubleDistance;

    private void start1()
    {
       
    }

    /*private void start2()
    {
        float[] f = new float[1] { 0f };
        Action[] a = new Action[1] { action1 };

        void action1()
        {
            Vector2 forceMultiplier = new Vector2(1.0f, 1.1f);
            Vector2 forceOffset = Vector2.zero;

            void config(Transform bullet) => bullet.localEulerAngles = Vector3.zero;

            Transform bullet = WeaponBehaviourHelper.SpawnAndShootBulletInArc(aim, forceMultiplier, forceOffset,
                weaponSystem, weaponConfiguration, side, config);

        }
    } 



    private void setup() {
        fastCouroutine = new FastCoroutine(f.Add(4), a.Add(terminate));
    }

    private void Update()
    {
        fastCouroutine.Update();
        Debug.Log(doubleDistance);
    }*/
}
