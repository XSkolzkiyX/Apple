using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    [Space(10)]

    [Header("Audio")]
    public float masterVolume = 1.0f;
    public int musicVolume = 20;
    public int effectsVolume = 20;
    [Space(10)]

    [Header("Controls")]
    public KeyCode moveForward = KeyCode.W;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveBackward = KeyCode.S;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode crouch = KeyCode.LeftControl;
    public KeyCode toggleCrouch = KeyCode.C;
    public KeyCode interact = KeyCode.F;
    public KeyCode use = KeyCode.E;

    private string savePath => Path.Combine(Application.persistentDataPath, "PlayerSettings.json");

    public void Save()
    {
        File.WriteAllText(savePath, JsonUtility.ToJson(this));
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            JsonUtility.FromJsonOverwrite(File.ReadAllText(savePath), this);
        }
    }
}
