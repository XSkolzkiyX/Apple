using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settings", menuName = "Data/Settings Data")]
public class SettingsData : ScriptableObject
{
    [Header("Video")]
    public int resolutionIndex = 2;
    public bool fullscreen = true;
    public bool vsync = false;
    public int frameLimit = -1;
    [Space(10)]

    [Header("Graphic")]
    public int qualityIndex = 2;
}
