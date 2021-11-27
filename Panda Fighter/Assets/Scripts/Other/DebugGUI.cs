using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public static string debugText1;
    public static string debugText2;
    public static string debugText3;
    public static string debugText4;
    public static string debugText5;
    public static string debugText6;
    public static string debugText7;
    public static string debugText8;
    public static string debugText9;
    public static string debugText10;

    private GUIStyle guiStyle = new GUIStyle();

    void OnEnable()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        int w = Screen.width;
        int h = Screen.height;

        GUI.Label(new Rect(1 * w / 10, 1 * h / 10, w / 10, h / 10), debugText1, guiStyle);
        GUI.Label(new Rect(6 * w / 10, 1 * h / 10, w / 10, h / 10), debugText2, guiStyle);
        GUI.Label(new Rect(1 * w / 10, 3 * h / 10, w / 10, h / 10), debugText3, guiStyle);
        GUI.Label(new Rect(6 * w / 10, 3 * h / 10, w / 10, h / 10), debugText4, guiStyle);
        GUI.Label(new Rect(1 * w / 10, 5 * h / 10, w / 10, h / 10), debugText5, guiStyle);
        GUI.Label(new Rect(6 * w / 10, 5 * h / 10, w / 10, h / 10), debugText6, guiStyle);
        GUI.Label(new Rect(1 * w / 10, 7 * h / 10, w / 10, h / 10), debugText7, guiStyle);
        GUI.Label(new Rect(6 * w / 10, 7 * h / 10, w / 10, h / 10), debugText8, guiStyle);
        GUI.Label(new Rect(1 * w / 10, 9 * h / 10, w / 10, h / 10), debugText9, guiStyle);
        GUI.Label(new Rect(6 * w / 10, 9 * h / 10, w / 10, h / 10), debugText10, guiStyle);
    }
}
