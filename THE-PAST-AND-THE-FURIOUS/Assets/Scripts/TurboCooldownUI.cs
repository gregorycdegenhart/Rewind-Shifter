using UnityEngine;
using UnityEngine.UI;

public class TurboCooldownUI : MonoBehaviour
{
    [Header("References")]
    public Image fillBar;
    public CarController carController;

    [Header("Colors")]
    public Color readyColor = Color.green;
    public Color cooldownColor = Color.yellow;
    public Color activeColor = Color.cyan;

    void Update()
    {
        if (fillBar == null || carController == null) return;

        if (carController.IsTurboActive())
        {
            fillBar.fillAmount = 1f;
            fillBar.color = activeColor;
        }
        else if (carController.GetCooldownProgress() > 0f)
        {
            fillBar.fillAmount = 1f - carController.GetCooldownProgress();
            fillBar.color = cooldownColor;
        }
        else
        {
            fillBar.fillAmount = 1f;
            fillBar.color = readyColor;
        }
    }
}
