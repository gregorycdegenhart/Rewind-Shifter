using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WinScreenUI : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI finalTimeText;

    void Start()
    {
        float raceTime = PlayerPrefs.GetFloat("RaceTime", 0f);

        int minutes = (int)(raceTime / 60f);
        int seconds = (int)(raceTime % 60f);
        int centiseconds = (int)((raceTime * 100f) % 100f);

        if (finalTimeText != null)
            finalTimeText.text = string.Format("Time: {0:00}:{1:00}.{2:00}", minutes, seconds, centiseconds);

        // Self-wire buttons
        WireButton("MenuButton", () => SceneManager.LoadScene("MainMenu"));
        WireButton("ReplayButton", () => SceneManager.LoadScene("Garage"));
    }

    void WireButton(string name, UnityEngine.Events.UnityAction action)
    {
        // Search in all canvases
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            Transform t = canvas.transform.Find(name);
            if (t != null)
            {
                Button btn = t.GetComponent<Button>();
                if (btn != null) btn.onClick.AddListener(action);
                return;
            }
        }
    }
}
