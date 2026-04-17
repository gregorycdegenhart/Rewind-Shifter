using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public HowToPlayScreen howToPlayScreen;

    void Start()
    {
        WireButton("PlayButton", () => SceneManager.LoadScene("Garage"));
        WireButton("QuitButton", () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        // Wire How To Play button
        if (howToPlayScreen != null)
            WireButton("HowToPlayButton", () => howToPlayScreen.Show());
    }

    void WireButton(string name, UnityEngine.Events.UnityAction action)
    {
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
