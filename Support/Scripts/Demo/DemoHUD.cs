//\$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved \$//\n
using UnityEngine;

public class DemoHUD : MonoBehaviour {

    public GUIStyle guiStyle;

    void OnGUI()
    {
        var bounds = new Rect(10, 10, 200, 70);
        GUI.Label(bounds, "Right Click - Look Around \nWASD - Move \nSpace - Generate New layout", guiStyle);
    }
}
