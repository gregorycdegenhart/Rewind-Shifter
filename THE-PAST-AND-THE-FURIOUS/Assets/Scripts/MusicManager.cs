using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoSpawn()
    {
        if (Instance != null) return;
        // Try to load from Resources, otherwise create empty
        var prefab = Resources.Load<MusicManager>("MusicManager");
        if (prefab != null)
            Instantiate(prefab);
        else
        {
            var go = new GameObject("MusicManager");
            go.AddComponent<MusicManager>();
        }
    }

    [Header("Playlist")]
    [Tooltip("Drag your music AudioClips here")]
    public AudioClip[] playlist;
    public string[] songNames;

    [Header("Settings")]
    public float volume = 0.5f;
    public bool shuffle = false;

    private AudioSource musicSource;
    private int currentTrackIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = false;
        musicSource.volume = volume;
        musicSource.spatialBlend = 0f;
        musicSource.playOnAwake = false;
    }

    void Start()
    {
        if (playlist != null && playlist.Length > 0)
            PlayTrack(currentTrackIndex);
    }

    void Update()
    {
        if (musicSource != null && !musicSource.isPlaying && playlist != null && playlist.Length > 0)
            NextTrack();
    }

    public void PlayTrack(int index)
    {
        if (playlist == null || playlist.Length == 0) return;

        currentTrackIndex = Mathf.Clamp(index, 0, playlist.Length - 1);
        musicSource.clip = playlist[currentTrackIndex];
        musicSource.Play();
    }

    public void NextTrack()
    {
        if (playlist == null || playlist.Length == 0) return;

        if (shuffle)
            currentTrackIndex = Random.Range(0, playlist.Length);
        else
            currentTrackIndex = (currentTrackIndex + 1) % playlist.Length;

        PlayTrack(currentTrackIndex);
    }

    public void PreviousTrack()
    {
        if (playlist == null || playlist.Length == 0) return;

        currentTrackIndex--;
        if (currentTrackIndex < 0)
            currentTrackIndex = playlist.Length - 1;

        PlayTrack(currentTrackIndex);
    }

    public void TogglePause()
    {
        if (musicSource.isPlaying)
            musicSource.Pause();
        else
            musicSource.UnPause();
    }

    public void SetVolume(float vol)
    {
        volume = Mathf.Clamp01(vol);
        musicSource.volume = volume;
    }

    public string GetCurrentSongName()
    {
        if (songNames != null && currentTrackIndex < songNames.Length && !string.IsNullOrEmpty(songNames[currentTrackIndex]))
            return songNames[currentTrackIndex];

        if (playlist != null && currentTrackIndex < playlist.Length && playlist[currentTrackIndex] != null)
            return playlist[currentTrackIndex].name;

        return "No Track";
    }

    public int GetCurrentTrackIndex() => currentTrackIndex;
    public int GetTrackCount() => playlist != null ? playlist.Length : 0;
    public bool IsPlaying() => musicSource != null && musicSource.isPlaying;
}
