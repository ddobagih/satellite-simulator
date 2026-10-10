using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public MissionManager missionManager;

    private AudioSource audioSource;
    private MissionPhase lastPhase = MissionPhase.Idle;
    private AudioClip clickClip;
    private AudioClip reserveClip;
    private AudioClip startClip;
    private AudioClip observeClip;
    private AudioClip transmitClip;
    private AudioClip completeClip;
    private AudioClip failureClip;
    private const float MasterVolume = 0.42f;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 0.75f;
        clickClip = Resources.Load<AudioClip>("OrbitShare/SFX_Click") ?? CreateTone("Click", 620f, 0.055f, 0.11f);
        reserveClip = Resources.Load<AudioClip>("OrbitShare/SFX_Reserve") ?? CreateTone("Reserve", 620f, 0.075f, 0.12f);
        startClip = Resources.Load<AudioClip>("OrbitShare/SFX_Start") ?? CreateChord("Start", 440f, 554.37f, 0.12f, 0.11f);
        observeClip = Resources.Load<AudioClip>("OrbitShare/SFX_Observe") ?? CreateTone("Observe", 740f, 0.075f, 0.1f);
        transmitClip = Resources.Load<AudioClip>("OrbitShare/SFX_Transmit") ?? CreateChord("Transmit", 493.88f, 659.25f, 0.13f, 0.1f);
        completeClip = Resources.Load<AudioClip>("OrbitShare/SFX_Complete") ?? CreateChord("Complete", 523.25f, 783.99f, 0.24f, 0.12f);
        failureClip = Resources.Load<AudioClip>("OrbitShare/SFX_Failure") ?? CreateChord("Failure", 392f, 293.66f, 0.26f, 0.11f);
    }

    private void OnEnable()
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged += HandleMissionChanged;
        }
    }

    private void OnDisable()
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= HandleMissionChanged;
        }
    }

    public void Attach(MissionManager mission)
    {
        if (missionManager != null)
        {
            missionManager.OnMissionChanged -= HandleMissionChanged;
        }

        missionManager = mission;
        lastPhase = mission != null ? mission.Phase : MissionPhase.Idle;

        if (missionManager != null)
        {
            missionManager.OnMissionChanged += HandleMissionChanged;
        }
    }

    public void PlayClick()
    {
        Play(clickClip, 0.65f, 1.03f);
    }

    private void HandleMissionChanged()
    {
        if (missionManager == null || missionManager.Phase == lastPhase)
        {
            return;
        }

        lastPhase = missionManager.Phase;
        switch (lastPhase)
        {
            case MissionPhase.Reserved:
                Play(reserveClip, 0.7f, 1.02f);
                break;
            case MissionPhase.Running:
                Play(startClip, 0.78f, 1f);
                break;
            case MissionPhase.Observing:
                Play(observeClip, 0.62f, 1.04f);
                break;
            case MissionPhase.Transmitting:
                Play(transmitClip, 0.72f, 1.01f);
                break;
            case MissionPhase.Complete:
                Play(completeClip, 0.86f, 1f);
                break;
            case MissionPhase.Failed:
                Play(failureClip, 0.78f, 0.96f);
                break;
        }
    }

    private void Play(AudioClip clip, float volumeScale, float pitch)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(clip, MasterVolume * volumeScale);
        }
    }

    private AudioClip CreateTone(string clipName, float frequency, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = GetSoftEnvelope(t, duration);
            samples[i] = Mathf.Sin(2f * Mathf.PI * frequency * t) * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateChord(string clipName, float frequencyA, float frequencyB, float duration, float volume)
    {
        int sampleRate = 44100;
        int sampleCount = Mathf.CeilToInt(sampleRate * duration);
        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)sampleRate;
            float envelope = GetSoftEnvelope(t, duration);
            float wave = Mathf.Sin(2f * Mathf.PI * frequencyA * t) + Mathf.Sin(2f * Mathf.PI * frequencyB * t);
            samples[i] = wave * 0.5f * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private float GetSoftEnvelope(float t, float duration)
    {
        float attack = Mathf.Clamp01(t / 0.018f);
        float release = Mathf.Clamp01((duration - t) / Mathf.Max(0.035f, duration * 0.45f));
        return Mathf.SmoothStep(0f, 1f, attack) * Mathf.SmoothStep(0f, 1f, release);
    }
}
