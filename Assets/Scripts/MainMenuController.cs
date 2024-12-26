using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider loadingProgressBarSlider;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator crossFadeAnimator;
    [Space(10)]

    [SerializeField] private List<string> tipTexts;

    private AsyncOperation loadingProgress;

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
