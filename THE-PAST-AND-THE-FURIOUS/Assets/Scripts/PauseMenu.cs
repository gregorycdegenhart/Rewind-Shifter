using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject howToPlayPanel;

    private bool isPaused = false;
    private CursorLockMode previousLockMode;
    private bool previousCursorVisible;

    void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            // Self-wire buttons by name - bulletproof, no persistent listeners needed
            WireButton(pausePanel, "ResumeButton", OnResumeButton);
            WireButton(pausePanel, "HTPPauseButton", OnHowToPlayButton);
            WireButton(pausePanel, "RestartButton", OnRestartButton);
            WireButton(pausePanel, "QuitButton", OnQuitToMenuButton);
        }
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            WireButton(howToPlayPanel, "CloseHTPButton", CloseHowToPlay);
        }
    }

    void Update()
    {
        bool pausePressed = false;

        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame ||
                UnityEngine.InputSystem.Keyboard.current.pKey.wasPressedThisFrame)
                pausePressed = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            pausePressed = true;

        if (pausePressed)
        {
            if (howToPlayPanel != null && howToPlayPanel.activeSelf)
                CloseHowToPlay();
            else
                TogglePause();
        }
    }

    public void TogglePause()
    {
        if (RaceManager.Instance != null && RaceManager.Instance.IsRaceFinished)
            return;

        isPaused = !isPaused;

        if (isPaused)
        {
            previousLockMode = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = previousLockMode;
            Cursor.visible = previousCursorVisible;
        }

        if (pausePanel != null)
            pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;
    }

    public void OnResumeButton() { if (isPaused) TogglePause(); }

    public void OnHowToPlayButton()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (pausePanel != null && isPaused) pausePanel.SetActive(true);
    }

    public void OnRestartButton()
    {
        Unpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitToMenuButton()
    {
        Unpause();
        SceneManager.LoadScene("MainMenu");
    }

    void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    static void WireButton(GameObject parent, string name, UnityEngine.Events.UnityAction action)
    {
        Transform t = parent.transform.Find(name);
        if (t == null) return;
        Button btn = t.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(action);
    }
}
