using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausePanelController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Animator crossFadeAnimator;

    [HideInInspector] public bool canPause = true;

    private HackAbility hackAbility;

    private void Start()
    {
        hackAbility = FindFirstObjectByType<HackAbility>();
    }

    private void Update()
    {
        if (!canPause) return;
        if(Input.GetKeyDown(KeyCode.Escape)) ChangePauseState();
    }

    private void ChangePauseState()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);

        GameManager.Instance.TogglePause();

        if(hackAbility)
        {
            if (pausePanel.activeSelf)
            {
                hackAbility?.DeactivateHackAbility();
                hackAbility.lockAbility = true;
            }
            else hackAbility.lockAbility = false;
        }

        //Time.timeScale = pausePanel.activeSelf ? 0f : 1f;
        //Cursor.lockState = pausePanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        //Cursor.visible = pausePanel.activeSelf;
    }

    public void OnResumeButtonClick()
    {
        ChangePauseState();
        //pausePanel.SetActive(false);
        //Time.timeScale = 1f;
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
    }

    public void OnBackToMenuButtonClick()
    {
        crossFadeAnimator.SetBool("Fade", true);
        canPause = false;
        StartCoroutine(LoadMainMenu());
    }

    private IEnumerator LoadMainMenu()
    {
        yield return new WaitForSecondsRealtime(.5f);
        SceneManager.LoadScene(0);
    }
}
