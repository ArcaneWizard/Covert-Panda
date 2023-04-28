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


    public static float[] f;
    public static Action[] a;

    private void Start()
    {
        if (f == null)
        {
            f = new float[500];
            for (int i = 0; i < 500; i++)
                f[i] = i * Time.deltaTime;
        }

        if (a == null)
        {
            a = new Action[500];
            for (int i = 0; i < 100; i++)
            {
                a[0 + 5 * i] = (bob);
                a[1 + 5 * i] = (joe);
                a[2 + 5 * i] = (cow);
                a[3 + 5 * i] = (dee);
                a[4 + 5 * i] = (nah);
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

           fastCouroutine = new TimedActions(f, a);
          
      // yield return StartCoroutine(testVillage());
    }

    private bool yay;
    private void finale()
    {
        yay = true;
    }

    private void Update()
    {
        fastCouroutine.Update();
    }

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
        for (int i = 0; i < 10000; i++)
        {
            yield return null;
            distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
            yield return null;
            doubleDistance = 4 * 2;
            yield return null;
            doubleDistance = 4 * distance;
            yield return null;
            doubleDistance = 4 * 3;
        }
    }

    private IEnumerator testVillage2()
    {
            distance = MathX.GetSquaredDistance(new Vector2(0, -1), new Vector2(-6, 7f));
            yield return null;
    }
    
    private TimedActions fastCouroutine;

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
