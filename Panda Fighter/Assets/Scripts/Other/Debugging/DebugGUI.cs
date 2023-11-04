using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public static string[] debugTexts;

    private GUIStyle guiStyle = new GUIStyle();

    void OnEnable()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.white;

        debugTexts = new string[8]; 
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[0], guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[1], guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[2], guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[3], guiStyle);
        GUI.Label(new Rect(Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[4], guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[5], guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[6], guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), debugTexts[7], guiStyle);
    }
}
