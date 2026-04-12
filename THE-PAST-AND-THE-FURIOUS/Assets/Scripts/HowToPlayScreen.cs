using UnityEngine;
using UnityEngine.UI;

public class HowToPlayScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject howToPlayPanel;

    void Start()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
            // Self-wire close button
            Transform closeBtn = howToPlayPanel.transform.Find("CloseHTPButton");
            if (closeBtn != null)
            {
                Button btn = closeBtn.GetComponent<Button>();
                if (btn != null) btn.onClick.AddListener(Close);
            }
        }
    }

    void Update()
    {
        if (howToPlayPanel != null && howToPlayPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Close();
        }
    }

    public void Show()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    public void Close()
    {
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(false);
    }
}
