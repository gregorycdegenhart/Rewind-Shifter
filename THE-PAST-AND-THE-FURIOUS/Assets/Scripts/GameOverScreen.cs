using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI reasonText;

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            WireButton(gameOverPanel, "RetryButton", OnRetryButton);
            WireButton(gameOverPanel, "GOQuitButton", OnQuitToMenuButton);
        }
    }

    public void ShowGameOver(string reason = "Race Over!")
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (reasonText != null)
            reasonText.text = reason;
        Time.timeScale = 0f;
    }

    public void OnRetryButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnQuitToMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    static void WireButton(GameObject parent, string name, UnityEngine.Events.UnityAction action)
    {
        Transform t = parent.transform.Find(name);
        if (t == null) return;
        Button btn = t.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(action);
    }
}
