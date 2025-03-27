using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("General")]
    public SettingsData playerSettings;
    public SettingsData defaultSettings;
    [Space(10)]

    [Header("Video Panel")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider frameLimitSlider;
    [SerializeField] private TextMeshProUGUI frameLimitText;
    public List<Vector2Int> resolutions;
    private int frameScale = 15;
    [Space(10)]

    [Header("Graphic Panel")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private void Start()
    {
        LoadOptions();
    }

    public void OnResolutionDropdownChanged()
    {
        Vector2Int currentResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(currentResolution.x, currentResolution.y, Screen.fullScreen);
    }

    public void OnFullscreenToggleChanged()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnVSyncToggle()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
    }

    public void OnFrameLimitSliderChanged()
    {
        int frameLimit = frameLimitSlider.value >= frameLimitSlider.maxValue ? -1 : (int)frameLimitSlider.value * frameScale;
        Application.targetFrameRate = frameLimit;
        frameLimitText.text = frameLimit < 0 ? "-" : frameLimit.ToString();
    }

    public void OnQualityChanged()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
    }

    public void SaveOptions()
    {
        playerSettings.resolutionIndex = resolutionDropdown.value;
        playerSettings.fullscreen = fullscreenToggle.isOn;
        playerSettings.vsync = vsyncToggle.isOn;
        playerSettings.frameLimit = frameLimitSlider.value >= frameLimitSlider.maxValue ? -1 : (int)frameLimitSlider.value * frameScale;

        playerSettings.qualityIndex = qualityDropdown.value;

        PlayerPrefs.SetInt("Resolution", playerSettings.resolutionIndex);
        PlayerPrefs.SetString("Fullscreen", playerSettings.fullscreen.ToString());
        PlayerPrefs.SetString("Vsync", playerSettings.vsync.ToString());
        PlayerPrefs.SetInt("Frame Limit", playerSettings.frameLimit);

        PlayerPrefs.SetInt("Quality", playerSettings.qualityIndex);
    }

    public void LoadOptions()
    {
        if (PlayerPrefs.HasKey("Resolution")) 
            playerSettings.resolutionIndex = PlayerPrefs.GetInt("Resolution");
        if (PlayerPrefs.HasKey("Fullscreen")) 
            playerSettings.fullscreen = bool.Parse(PlayerPrefs.GetString("Fullscreen"));
        if (PlayerPrefs.HasKey("Vsync"))
            playerSettings.vsync = bool.Parse(PlayerPrefs.GetString("Vsync"));
        if (PlayerPrefs.HasKey("Frame Limit"))
            playerSettings.frameLimit = PlayerPrefs.GetInt("Frame Limit");
        if (PlayerPrefs.HasKey("Quality"))
            playerSettings.qualityIndex = PlayerPrefs.GetInt("Quality");

        FillPlayerData();
    }

    private void FillPlayerData()
    {
        resolutionDropdown.value = playerSettings.resolutionIndex;
        fullscreenToggle.isOn = playerSettings.fullscreen;
        vsyncToggle.isOn = playerSettings.vsync;
        frameLimitSlider.value = playerSettings.frameLimit > 0 ? playerSettings.frameLimit / frameScale : frameLimitSlider.maxValue;
        frameLimitText.text = playerSettings.frameLimit < 0 ? "-" : playerSettings.frameLimit.ToString();

        qualityDropdown.value = playerSettings.qualityIndex;
    }

    public void ResetOptions()
    {
        playerSettings.resolutionIndex = defaultSettings.resolutionIndex;
        playerSettings.fullscreen = defaultSettings.fullscreen;
        playerSettings.vsync= defaultSettings.vsync;
        playerSettings.frameLimit = defaultSettings.frameLimit;

        playerSettings.qualityIndex = defaultSettings.qualityIndex;
        FillPlayerData();
    }
}
