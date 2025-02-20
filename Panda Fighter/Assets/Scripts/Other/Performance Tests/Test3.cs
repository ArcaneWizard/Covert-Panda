/*using System;
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

public class Test3 : MonoBehaviour
{
    private int i;

    float distance;
    float doubleDistance;

    public List<ExecutionDelay> f;
    public List<Action> a;

    private TimedCode fastCouroutine;
    void Start()
    {
        f = new List<ExecutionDelay>();
        a = new List<Action>();
       
         fastCouroutine = new TimedCode(f, a);
        f.Clear();
        for (int i = 0; i < 1; i++)
            f.Add(new ExecutionDelay(0f));

        a.Clear();
        for (int i = 0; i < 1; i++)
        {
            a.Add(bob);
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

            //fastCouroutine.Start();
        }
        
        //fastCouroutine.Start();
    }

    private void Update()
    {
        i++;
        //fastCouroutine.Update();
    }
}*/