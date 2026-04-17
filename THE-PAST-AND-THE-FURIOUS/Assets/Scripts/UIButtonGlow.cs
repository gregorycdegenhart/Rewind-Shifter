using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonGlow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Glow Settings")]
    public Color glowColor = new Color(1f, 0.15f, 0.15f, 0.9f);
    public float glowWidth = 4f;

    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline == null)
            outline = gameObject.AddComponent<Outline>();

        outline.effectColor = glowColor;
        outline.effectDistance = new Vector2(glowWidth, glowWidth);
        outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}
