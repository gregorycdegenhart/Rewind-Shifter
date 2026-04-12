using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicPlaylistUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI trackNumberText;
    public GameObject panel;

    [Header("Volume")]
    public Slider volumeSlider;

    [Header("Auto-Hide")]
    public float showDuration = 4f;
    public bool alwaysVisible = false;

    private float showTimer = 0f;
    private bool isVisible = false;
    private string lastSongName = "";
    private TextMeshProUGUI playPauseText;

    void Start()
    {
        // Self-wire buttons
        if (panel != null)
        {
            panel.SetActive(true); // start visible in menus
            WireButton("PrevTrackBtn", OnPreviousButton);
            WireButton("PlayPauseBtn", OnPlayPauseButton);
            WireButton("NextTrackBtn", OnNextButton);

            // Grab play/pause button text for toggling label
            var ppBtn = panel.transform.Find("PlayPauseBtn");
            if (ppBtn != null)
                playPauseText = ppBtn.GetComponentInChildren<TextMeshProUGUI>();

            // Auto-find text if not assigned
            if (songNameText == null)
            {
                var t = panel.transform.Find("SongNameText");
                if (t != null) songNameText = t.GetComponent<TextMeshProUGUI>();
            }
            if (trackNumberText == null)
            {
                var t = panel.transform.Find("TrackNumberText");
                if (t != null) trackNumberText = t.GetComponent<TextMeshProUGUI>();
            }

            // Auto-find volume slider
            if (volumeSlider == null)
            {
                var t = panel.transform.Find("VolumeSlider");
                if (t != null) volumeSlider = t.GetComponent<Slider>();
            }
        }

        // Wire volume slider
        if (volumeSlider != null)
        {
            if (MusicManager.Instance != null)
                volumeSlider.value = MusicManager.Instance.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        ShowWidget();
    }

    void Update()
    {
        if (MusicManager.Instance == null) return;

        string currentSong = MusicManager.Instance.GetCurrentSongName();

        if (currentSong != lastSongName)
        {
            lastSongName = currentSong;
            ShowWidget();
        }

        // Only auto-hide during gameplay, not in menus or when alwaysVisible
        if (isVisible && !alwaysVisible && RaceManager.Instance != null)
        {
            showTimer -= Time.unscaledDeltaTime;
            if (showTimer <= 0f)
                HideWidget();
        }

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (MusicManager.Instance == null) return;

        if (songNameText != null)
            songNameText.text = MusicManager.Instance.GetCurrentSongName();

        if (trackNumberText != null)
        {
            int current = MusicManager.Instance.GetCurrentTrackIndex() + 1;
            int total = MusicManager.Instance.GetTrackCount();
            trackNumberText.text = current + " / " + total;
        }
    }

    void ShowWidget()
    {
        if (panel != null)
            panel.SetActive(true);
        isVisible = true;
        showTimer = showDuration;
    }

    void HideWidget()
    {
        if (panel != null)
            panel.SetActive(false);
        isVisible = false;
    }

    public void OnNextButton()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.NextTrack();
            UpdatePlayPauseLabel();
            ShowWidget();
        }
    }

    public void OnPreviousButton()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.PreviousTrack();
            UpdatePlayPauseLabel();
            ShowWidget();
        }
    }

    public void OnPlayPauseButton()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.TogglePause();
            UpdatePlayPauseLabel();
            ShowWidget();
        }
    }

    void UpdatePlayPauseLabel()
    {
        if (playPauseText == null || MusicManager.Instance == null) return;
        playPauseText.text = MusicManager.Instance.IsPaused() ? ">" : "||";
    }

    public void OnVolumeChanged(float value)
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.SetVolume(value);
    }

    public void OnToggleWidget()
    {
        if (isVisible) HideWidget();
        else ShowWidget();
    }

    void WireButton(string name, UnityEngine.Events.UnityAction action)
    {
        Transform t = panel.transform.Find(name);
        if (t == null) return;
        Button btn = t.GetComponent<Button>();
        if (btn != null) btn.onClick.AddListener(action);
    }
}
