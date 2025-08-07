using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Unity.VisualScripting.Member;
using static UnityEngine.GraphicsBuffer;

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
    [Space(10)]

    [Header("Controls")]
    [SerializeField] private GameObject rebindPanel;
    [SerializeField] private TextMeshProUGUI moveForwardText;
    [SerializeField] private TextMeshProUGUI moveLeftText;
    [SerializeField] private TextMeshProUGUI moveBackwardText;
    [SerializeField] private TextMeshProUGUI moveRightText;
    [SerializeField] private TextMeshProUGUI crouchText;
    [SerializeField] private TextMeshProUGUI toggleCrouchText;
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private TextMeshProUGUI useText;

    private bool detectKey = false;
    private KeyCode detectedKey;
    private string bindKey;
    
    private void Start()
    {
        LoadOptions();
    }

    private void Update()
    {
        if (!detectKey) return;
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key is KeyCode.Escape)
                    {
                        CancelControlKeyBind();
                        return;
                    }
                    detectedKey = key;
                    detectKey = false;
                    CompleteControlKeyBind();
                    return;
                }
            }
        }
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

    public void OnSetControlKeyButtonClick(string key)
    {
        detectedKey = KeyCode.None;
        detectKey = true;
        bindKey = key;
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

        playerSettings.Save();
    }

    public void LoadOptions()
    {
        playerSettings.Load();

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

        moveForwardText.text = playerSettings.moveForward.ToString();
        moveLeftText.text = playerSettings.moveLeft.ToString();
        moveBackwardText.text = playerSettings.moveBackward.ToString();
        moveRightText.text = playerSettings.moveRight.ToString();
        crouchText.text = playerSettings.crouch.ToString();
        toggleCrouchText.text = playerSettings.toggleCrouch.ToString();
        interactText.text = playerSettings.interact.ToString();
        useText.text = playerSettings.use.ToString();
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

        playerSettings.moveForward = defaultSettings.moveForward;
        playerSettings.moveLeft = defaultSettings.moveLeft;
        playerSettings.moveBackward = defaultSettings.moveBackward;
        playerSettings.moveRight = defaultSettings.moveRight;
        playerSettings.crouch = defaultSettings.crouch;
        playerSettings.toggleCrouch = defaultSettings.toggleCrouch;
        playerSettings.interact = defaultSettings.interact;
        playerSettings.use = defaultSettings.use;

        FillPlayerData();
    }

    private void CompleteControlKeyBind()
    {
        rebindPanel.SetActive(false);

        switch (bindKey)
        {
            case "Forward":
                playerSettings.moveForward = detectedKey;
                moveForwardText.text = detectedKey.ToString();
                break;
            case "Left":
                playerSettings.moveLeft = detectedKey;
                moveLeftText.text = detectedKey.ToString();
                break;
            case "Back":
                playerSettings.moveBackward = detectedKey;
                moveBackwardText.text = detectedKey.ToString();
                break;
            case "Right":
                playerSettings.moveRight = detectedKey;
                moveRightText.text = detectedKey.ToString();
                break;
            case "Crouch":
                playerSettings.crouch = detectedKey;
                crouchText.text = detectedKey.ToString();
                break;
            case "ToggleCrouch":
                playerSettings.toggleCrouch = detectedKey;
                toggleCrouchText.text = detectedKey.ToString();
                break;
            case "Interact":
                playerSettings.interact = detectedKey;
                interactText.text = detectedKey.ToString();
                break;
            case "Use":
                playerSettings.use = detectedKey;
                useText.text = detectedKey.ToString();
                break;
        }
    }

    private void CancelControlKeyBind()
    {
        detectKey = false;
        detectedKey = KeyCode.None;
        rebindPanel.SetActive(false);
    }
}
