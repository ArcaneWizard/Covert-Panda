using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static Validation;

public class TestInstantiate : MonoBehaviour
{
    [SerializeField] private GameObject testObject;
    [SerializeField] private Transform lol;
    [SerializeField] private Transform boba;
    [SerializeField] private Transform arcane;
    [SerializeField] private Transform secondTestObject;
    [SerializeField] private int numOfObjects = 100000;

    void OnValidate()
    {
        this.RequireTag(lol, nameof(lol), "Joe");
        this.ConfirmNotNull(boba, nameof(boba));
    }

    void Start()
    {

        float time = Time.time;
        for (int i = 0; i < numOfObjects; i++) 
            Instantiate(testObject, transform.position, Quaternion.identity, transform);

        Debug.Log("time elapsed: " + (Time.time - time));
    }
}
