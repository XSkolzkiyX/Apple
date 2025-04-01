using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
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
    [Space(10)]

    [Header("Audio")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private AudioMixer main;

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

    public void OnMasterVolumeChanged()
    {
        AudioListener.volume = masterVolumeSlider.value;
        //masterVolumeText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100).ToString();
        masterVolumeText.text = Math.Round(masterVolumeSlider.value, 1).ToString();
    }

    public void OnMusicVolumeChanged()
    {
        main.SetFloat("Music", musicVolumeSlider.value);
        double volume = Math.Round((musicVolumeSlider.value - musicVolumeSlider.minValue) / (musicVolumeSlider.maxValue - musicVolumeSlider.minValue), 1);
        musicVolumeText.text = volume.ToString();
    }

    public void OnEffectsVolumeChanged()
    {
        main.SetFloat("Effects", effectsVolumeSlider.value);
        double volume = Math.Round((effectsVolumeSlider.value - effectsVolumeSlider.minValue) / (effectsVolumeSlider.maxValue - effectsVolumeSlider.minValue), 1);
        effectsVolumeText.text = volume.ToString();
    }

    public void SaveOptions()
    {
        playerSettings.resolutionIndex = resolutionDropdown.value;
        playerSettings.fullscreen = fullscreenToggle.isOn;
        playerSettings.vsync = vsyncToggle.isOn;
        playerSettings.frameLimit = frameLimitSlider.value >= frameLimitSlider.maxValue ? -1 : (int)frameLimitSlider.value * frameScale;

        playerSettings.qualityIndex = qualityDropdown.value;

        playerSettings.masterVolume = masterVolumeSlider.value;
        playerSettings.musicVolume = (int)musicVolumeSlider.value;
        playerSettings.effectsVolume = (int)effectsVolumeSlider.value;

        PlayerPrefs.SetInt("Resolution", playerSettings.resolutionIndex);
        PlayerPrefs.SetString("Fullscreen", playerSettings.fullscreen.ToString());
        PlayerPrefs.SetString("Vsync", playerSettings.vsync.ToString());
        PlayerPrefs.SetInt("Frame Limit", playerSettings.frameLimit);

        PlayerPrefs.SetInt("Quality", playerSettings.qualityIndex);

        PlayerPrefs.SetFloat("MasterVolume", playerSettings.masterVolume);
        PlayerPrefs.SetInt("MusicVolume", playerSettings.musicVolume);
        PlayerPrefs.SetInt("EffectsVolume", playerSettings.effectsVolume);
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
        if (PlayerPrefs.HasKey("MasterVolume"))
            playerSettings.masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        if (PlayerPrefs.HasKey("MusicVolume"))
            playerSettings.musicVolume = PlayerPrefs.GetInt("MusicVolume");
        if (PlayerPrefs.HasKey("EffectsVolume"))
            playerSettings.effectsVolume = PlayerPrefs.GetInt("EffectsVolume");

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

        masterVolumeSlider.value = playerSettings.masterVolume;
        musicVolumeSlider.value = playerSettings.musicVolume;
        effectsVolumeSlider.value = playerSettings.effectsVolume;
    }

    public void ResetOptions()
    {
        playerSettings.resolutionIndex = defaultSettings.resolutionIndex;
        playerSettings.fullscreen = defaultSettings.fullscreen;
        playerSettings.vsync= defaultSettings.vsync;
        playerSettings.frameLimit = defaultSettings.frameLimit;

        playerSettings.qualityIndex = defaultSettings.qualityIndex;

        playerSettings.masterVolume = defaultSettings.masterVolume;
        playerSettings.musicVolume = defaultSettings.musicVolume;
        playerSettings.effectsVolume = defaultSettings.effectsVolume;
        FillPlayerData();
    }
}
