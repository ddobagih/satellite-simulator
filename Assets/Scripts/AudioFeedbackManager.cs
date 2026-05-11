using UnityEngine;

public class AudioFeedbackManager : MonoBehaviour
{
    public MissionManager missionManager;

    private AudioSource audioSource;
    private MissionPhase lastPhase = MissionPhase.Idle;
    private AudioClip reserveClip;
    private AudioClip startClip;
    private AudioClip observeClip;
    private AudioClip transmitClip;
    private AudioClip completeClip;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        reserveClip = Resources.Load<AudioClip>("OrbitShare/SFX_Reserve") ?? CreateTone("Reserve", 660f, 0.08f, 0.18f);
        startClip = Resources.Load<AudioClip>("OrbitShare/SFX_Start") ?? CreateTone("Start", 440f, 0.1f, 0.16f);
        observeClip = Resources.Load<AudioClip>("OrbitShare/SFX_Observe") ?? CreateTone("Observe", 880f, 0.07f, 0.14f);
        transmitClip = Resources.Load<AudioClip>("OrbitShare/SFX_Transmit") ?? CreateTone("Transmit", 520f, 0.08f, 0.16f);
        completeClip = Resources.Load<AudioClip>("OrbitShare/SFX_Complete") ?? CreateChord("Complete", 523.25f, 659.25f, 0.35f, 0.18f);
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
                Play(reserveClip);
                break;
            case MissionPhase.Running:
                Play(startClip);
                break;
            case MissionPhase.Observing:
                Play(observeClip);
                break;
            case MissionPhase.Transmitting:
                Play(transmitClip);
                break;
            case MissionPhase.Complete:
                Play(completeClip);
                break;
        }
    }

    private void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip, 0.55f);
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
            float envelope = Mathf.Clamp01(1f - t / duration);
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
            float envelope = Mathf.Clamp01(1f - t / duration);
            float wave = Mathf.Sin(2f * Mathf.PI * frequencyA * t) + Mathf.Sin(2f * Mathf.PI * frequencyB * t);
            samples[i] = wave * 0.5f * envelope * volume;
        }

        AudioClip clip = AudioClip.Create(clipName, sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
