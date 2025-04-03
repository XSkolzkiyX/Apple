using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class MainMenuController : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Slider loadingProgressBarSlider;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator crossFadeAnimator;
    [SerializeField] private List<string> tipTexts;
    [Space(10)]

    [Header("Settings")]
    [Header("Video Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider frameLimitSlider;
    [SerializeField] private TextMeshProUGUI frameLimitText;
    [SerializeField] private Vector2Int[] resolutions;
    [Space(10)]
    [Header("Graphic Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [Space(10)]
    [Header("Audio Settings")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider effectsVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI effectsVolumeText;
    [SerializeField] private AudioMixer main;
    [Space(10)]
    [Header("Controls Settings")]
    [SerializeField] private InputSettings playerControls;
    [SerializeField] private InputSettings defaultControls;
    [SerializeField] private List<TextMeshProUGUI> controlKeyTexts;
    [Space(10)]


    private AsyncOperation loadingProgress;
    private int frameScale = 30;
    private bool detectKey = false;
    private KeyCode detectedKey;
    private int bindKeyIndex;
    private readonly string[] keyPropertyNames =
{
    "sprintKey", "interactKey", "dropKey", "jumpKey",
    "pauseKey", "shootKey", "aimKey", "reloadKey", "hackKey"
};

    private void Start()
    {
        Time.timeScale = 1.0f;
        LoadSettings();
        FillControlPanel();
    }

    private void Update()
    {
        if (!detectKey) return;
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    detectedKey = key;
                    detectKey = false;
                    CompleteControlKeyBind();
                    return;
                }
            }
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("Resolution", resolutionDropdown.value);
        PlayerPrefs.SetString("Fullscreen", fullscreenToggle.isOn.ToString());
        PlayerPrefs.SetString("VSync", vsyncToggle.isOn.ToString());
        PlayerPrefs.SetFloat("FrameLimit", frameLimitSlider.value);
        PlayerPrefs.SetInt("Quality", qualityDropdown.value);
        PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", effectsVolumeSlider.value);
    }

    private void LoadSettings()
    {
        if(PlayerPrefs.HasKey("Resolution")) resolutionDropdown.value = PlayerPrefs.GetInt("Resolution");
        if (PlayerPrefs.HasKey("Fullscreen")) fullscreenToggle.isOn = bool.Parse(PlayerPrefs.GetString("Fullscreen"));
        if(PlayerPrefs.HasKey("VSync"))vsyncToggle.isOn = bool.Parse(PlayerPrefs.GetString("VSync"));
        if(PlayerPrefs.HasKey("FrameLimit"))frameLimitSlider.value = PlayerPrefs.GetFloat("FrameLimit");
        if(PlayerPrefs.HasKey("Quality"))qualityDropdown.value = PlayerPrefs.GetInt("Quality");
        if(PlayerPrefs.HasKey("MasterVolume"))masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        if(PlayerPrefs.HasKey("MusicVolume"))musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        if (PlayerPrefs.HasKey("EffectsVolume")) effectsVolumeSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
    }

    public void OnPlayButtonClick(int sceneIndex)
    {
        tipText.text = tipTexts[Random.Range(0, tipTexts.Count)];
        loadingScreen.SetActive(true);
        crossFadeAnimator.SetBool("Fade", true);
        StartCoroutine(LoadScene(.5f, sceneIndex));
    }

    public void OnQuitButtonClick()
    {
        loadingScreen.SetActive(false);
        crossFadeAnimator.SetBool("Fade", true);
        Invoke(nameof(ApplicationQuit), 1f);
    }

    public void OnSaveSettingsButtonClick()
    {
        SaveSettings();
    }

    public void OnResetButtonClick()
    {
        playerControls = defaultControls;
        FillControlPanel();
    }

    public void OnResolutionChanged()
    {
        Vector2Int selectedResolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(selectedResolution.x, selectedResolution.y, Screen.fullScreen);
    }

    public void OnFullScreenToggle()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnVSyncToggle()
    {
        QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;
    }

    public void OnFrameLimitChanged()
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
        masterVolumeText.text = masterVolumeSlider.value.ToString();
        float masterVolume = masterVolumeSlider.value / 100.0f;
        AudioListener.volume = masterVolume;
    }

    public void OnMusicVolumeChanged()
    {
        musicVolumeText.text = musicVolumeSlider.value.ToString();
        float musicVolume = musicVolumeSlider.value > 0 ? Mathf.Log10(musicVolumeSlider.value / 100.0f) * 20.0f : -80.0f;
        main.SetFloat("MusicVolume", musicVolume);
    }

    public void OnEffectsVolumeChanged()
    {
        effectsVolumeText.text = effectsVolumeSlider.value.ToString();
        float sfxVolume = effectsVolumeSlider.value > 0 ? Mathf.Log10(effectsVolumeSlider.value / 100.0f) * 20.0f : -80.0f;
        main.SetFloat("SFXVolume", sfxVolume);
    }

    public void OnSetControlKeyButtonClick(int keyIndex)
    {
        detectedKey = KeyCode.None;
        detectKey = true;
        bindKeyIndex = keyIndex;
    }

    //private void CompleteControlKeyBind()
    //{
    //    if (bindKeyIndex < 0 || bindKeyIndex >= keyPropertyNames.Length) return;
    //
    //    var property = typeof(InputSettings).GetProperty(keyPropertyNames[bindKeyIndex]);
    //    property?.SetValue(playerControls, detectedKey);
    //
    //    FillControlPanel();
    //}

    private void CompleteControlKeyBind()
    {
        switch (bindKeyIndex)
        {
            case 0:
                playerControls.sprintKey = detectedKey;
                break;
            case 1:
                playerControls.interactKey = detectedKey;
                break;
            case 2:
                playerControls.dropKey = detectedKey;
                break;
            case 3:
                playerControls.jumpKey = detectedKey;
                break;
            case 4:
                playerControls.pauseKey = detectedKey;
                break;
            case 5:
                playerControls.shootKey = detectedKey;
                break;
            case 6:
                playerControls.aimKey = detectedKey; 
                break;
            case 7:
                playerControls.reloadKey = detectedKey;
                break;
            case 8:
                playerControls.hackKey = detectedKey;
                break;
        }
        FillControlPanel();
    }

    private void FillControlPanel()
    {
        //for (int i = 0; i < keyPropertyNames.Length; i++)
        //{
        //    var property = typeof(InputSettings).GetProperty(keyPropertyNames[i]);
        //    if (property != null)
        //    {
        //        controlKeyTexts[i].text = property.GetValue(playerControls).ToString();
        //    }
        //}

        //controlKeyTexts[0].text = playerControls.sprintKey.ToString();
        //controlKeyTexts[1].text = playerControls.interactKey.ToString();
        //controlKeyTexts[2].text = playerControls.dropKey.ToString();
        //controlKeyTexts[3].text = playerControls.jumpKey.ToString();
        //controlKeyTexts[4].text = playerControls.pauseKey.ToString();
        //controlKeyTexts[5].text = playerControls.shootKey.ToString();
        //controlKeyTexts[6].text = playerControls.aimKey.ToString();
        //controlKeyTexts[7].text = playerControls.reloadKey.ToString();
        //controlKeyTexts[8].text = playerControls.hackKey.ToString();
    }

    private void ApplicationQuit()
    {
        Application.Quit();
    }

    private IEnumerator LoadScene(float delay, int sceneIndex)
    {
        yield return new WaitForSeconds(delay);
        loadingProgress = SceneManager.LoadSceneAsync(sceneIndex);
        while(!loadingProgress.isDone)
        {
            loadingProgressBarSlider.value = loadingProgress.progress / .9f;
            yield return null;
        }
    }
}
