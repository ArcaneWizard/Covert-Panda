using UnityEngine;

public class DebugGUI : MonoBehaviour
{
    public static string[] DebugTexts;

    private GUIStyle guiStyle = new GUIStyle();

    void OnEnable()
    {
        guiStyle.fontSize = 40;
        guiStyle.normal.textColor = Color.white;

        DebugTexts = new string[8];
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[0], guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[1], guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[2], guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[3], guiStyle);
        GUI.Label(new Rect(Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[4], guiStyle);
        GUI.Label(new Rect(4 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[5], guiStyle);
        GUI.Label(new Rect(6 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[6], guiStyle);
        GUI.Label(new Rect(8 * Screen.width / 10, 8 * Screen.height / 10, Screen.width / 10, Screen.height / 10), DebugTexts[7], guiStyle);
    }
}
