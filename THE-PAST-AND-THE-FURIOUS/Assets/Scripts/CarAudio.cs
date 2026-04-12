using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [Header("Engine")]
    public float minPitch = 0.6f;
    public float maxPitch = 2.0f;
    public float engineVolume = 0.3f;

    [Header("SFX")]
    public float turboVolume = 0.5f;
    public float checkpointVolume = 0.6f;

    private CarController carController;
    private AudioSource engineSource;
    private AudioSource turboSource;
    private AudioSource sfxSource;
    private bool wasTurboActive = false;

    void Awake()
    {
        carController = GetComponent<CarController>();

        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = GenerateEngineClip();
        engineSource.loop = true;
        engineSource.volume = engineVolume;
        engineSource.spatialBlend = 0f;
        engineSource.Play();

        turboSource = gameObject.AddComponent<AudioSource>();
        turboSource.clip = GenerateTurboClip();
        turboSource.loop = false;
        turboSource.volume = turboVolume;
        turboSource.spatialBlend = 0f;
        turboSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.clip = GenerateCheckpointClip();
        sfxSource.loop = false;
        sfxSource.volume = checkpointVolume;
        sfxSource.spatialBlend = 0f;
        sfxSource.playOnAwake = false;
    }

    void Update()
    {
        if (carController == null) return;

        // engine pitch based on speed
        float speed = carController.rb.linearVelocity.magnitude;
        float t = Mathf.Clamp01(speed / carController.maxForwardSpeed);
        engineSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);

        // turbo whoosh
        bool turboActive = carController.IsTurboActive();
        if (turboActive && !wasTurboActive)
            turboSource.Play();
        wasTurboActive = turboActive;
    }

    public void PlayCheckpointSound()
    {
        if (sfxSource != null)
            sfxSource.Play();
    }

    static AudioClip GenerateEngineClip()
    {
        int sampleRate = 44100;
        int length = sampleRate; // 1 second
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float time = (float)i / sampleRate;
            // Smooth sine-based engine tone - deep rumble, not harsh
            float sample = 0f;
            sample += Mathf.Sin(2f * Mathf.PI * 75f * time) * 0.4f;        // deep fundamental
            sample += Mathf.Sin(2f * Mathf.PI * 150f * time) * 0.25f;       // first harmonic
            sample += Mathf.Sin(2f * Mathf.PI * 225f * time) * 0.1f;        // second harmonic
            sample += Mathf.Sin(2f * Mathf.PI * 300f * time) * 0.05f;       // subtle overtone
            // light noise for character, not harshness
            sample += (Random.value * 2f - 1f) * 0.02f;
            samples[i] = sample * 0.35f;
        }

        AudioClip clip = AudioClip.Create("Engine", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    static AudioClip GenerateTurboClip()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.8f); // 0.8 seconds
        float[] samples = new float[length];

        for (int i = 0; i < length; i++)
        {
            float time = (float)i / sampleRate;
            float progress = (float)i / length;

            // falling amplitude envelope
            float envelope = 1f - progress;
            envelope *= envelope;

            // white noise base
            float sample = (Random.value * 2f - 1f) * 0.6f;

            // sine sweep from 2000 Hz down to 500 Hz
            float freq = Mathf.Lerp(2000f, 500f, progress);
            sample += Mathf.Sin(2f * Mathf.PI * freq * time) * 0.4f;

            samples[i] = sample * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Turbo", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    static AudioClip GenerateCheckpointClip()
    {
        int sampleRate = 44100;
        int length = (int)(sampleRate * 0.3f); // 0.3 seconds
        float[] samples = new float[length];
        int half = length / 2;

        for (int i = 0; i < length; i++)
        {
            float time = (float)i / sampleRate;
            float freq = i < half ? 880f : 1760f;

            // quick decay within each tone
            float localProgress = i < half ? (float)i / half : (float)(i - half) / half;
            float envelope = 1f - localProgress * 0.5f;

            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * time) * envelope * 0.5f;
        }

        AudioClip clip = AudioClip.Create("Checkpoint", length, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    static float Sawtooth(float time, float frequency)
    {
        return 2f * ((time * frequency) % 1f) - 1f;
    }
}
