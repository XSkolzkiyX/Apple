using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0 : 1;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = pause;
    }

    public void TogglePause()
    {
        SetPause(!IsPaused);
    }
}
