using UnityEngine;
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
    }
}
