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
    public Vector2 bounds = new Vector2(-1, 1);

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        guiStyle.fontSize = 8;
        guiStyle.normal.textColor = Color.white;

        transform.name = transform.GetSiblingIndex().ToString();
        Handles.Label(transform.position + new Vector3(0, 3f, 0), transform.GetSiblingIndex().ToString(), guiStyle);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + new Vector3(bounds.x, transform.right.y / transform.right.x * bounds.x, 0), 0.3f);
        Gizmos.DrawSphere(transform.position + new Vector3(bounds.y, transform.right.y / transform.right.x * bounds.y, 0), 0.3f);
    }
#endif
}