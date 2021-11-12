using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
    using UnityEditor;
#endif


public class DecisionZoneName : MonoBehaviour
{
    private GUIStyle guiStyle = new GUIStyle();

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        guiStyle.fontSize = 8;
        guiStyle.normal.textColor = Color.white;

        transform.name = transform.GetSiblingIndex().ToString();
        Handles.Label(transform.position + new Vector3(0, 3f, 0), transform.GetSiblingIndex().ToString(), guiStyle);
    }
    #endif
}