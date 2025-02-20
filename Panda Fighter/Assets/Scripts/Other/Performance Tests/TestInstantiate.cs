/*using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Validation;

public class TestInstantiate : MonoBehaviour
{
    public GameObject testObject;
    public int numOfObjects;

    void Start()
    {

        float time = Time.time;
        for (int i = 0; i < numOfObjects; i++) 
            Instantiate(testObject, transform.position, Quaternion.identity, transform);
    }
}
*/