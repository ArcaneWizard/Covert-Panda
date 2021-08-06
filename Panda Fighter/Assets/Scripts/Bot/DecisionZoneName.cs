using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DecisionZoneName : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, transform.name);
    }
}