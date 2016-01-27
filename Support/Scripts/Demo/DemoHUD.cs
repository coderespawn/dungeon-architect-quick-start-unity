using UnityEngine;
using System.Collections;

public class DemoHUD : MonoBehaviour {

    public GUIStyle guiStyle;

    void OnGUI()
    {
        var bounds = new Rect(10, 10, 200, 70);
        GUI.Label(bounds, "Right Click - Look Around \nWASD - Move \nSpace - Generate New layout", guiStyle);
    }
}
