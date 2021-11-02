using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class DecisionZoneName : MonoBehaviour
{
    private GUIStyle guiStyle = new GUIStyle();

    void OnDrawGizmos()
    {
        guiStyle.fontSize = 8;
        guiStyle.normal.textColor = Color.white;

        transform.name = transform.GetSiblingIndex().ToString();
        Handles.Label(transform.position + new Vector3(0, 3f, 0), transform.GetSiblingIndex().ToString(), guiStyle);
    }
}