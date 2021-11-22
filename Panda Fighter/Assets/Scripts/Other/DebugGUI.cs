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

    private GUIStyle guiStyle = new GUIStyle();

    void OnEnable()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText1, guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText2, guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText3, guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText4, guiStyle);
        GUI.Label(new Rect(Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText5, guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText6, guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText7, guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugText8, guiStyle);
    }
}
