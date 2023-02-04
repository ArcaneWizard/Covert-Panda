using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInstantiate : MonoBehaviour
{
    [SerializeField] private GameObject testObject;
    [SerializeField] private int numOfObjects;

    void Start()
    {
        float time = Time.time;
        for (int i = 0; i < numOfObjects; i++) 
            Instantiate(testObject, transform.position, Quaternion.identity, transform);

        Debug.Log("time elapsed: " + (Time.time - time));
    }
}
