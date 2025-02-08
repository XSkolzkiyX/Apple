using DG.Tweening;
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

    [SerializeField] private List<string> tipTexts;

    private AsyncOperation loadingProgress;
    private int frameScale = 30;

    private void Start()
    {
        Time.timeScale = 1.0f;
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
        QualitySettings.vSyncCount = vsyncToggle ? 1 : 0;
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
