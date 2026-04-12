using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IScrollHandler
{
    public enum ClickType { Normal, Confirm, Back }

    [Tooltip("Which click sound to play")]
    public ClickType clickType = ClickType.Normal;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIAudioManager.Instance == null) return;

        switch (clickType)
        {
            case ClickType.Confirm:
                UIAudioManager.Instance.PlayConfirm();
                break;
            case ClickType.Back:
                UIAudioManager.Instance.PlayBack();
                break;
            default:
                UIAudioManager.Instance.PlayClick();
                break;
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayScroll();
    }
}
