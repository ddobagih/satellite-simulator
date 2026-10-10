using UnityEngine;

public class PulseScale : MonoBehaviour
{
    public SimulationManager simulationManager;
    public float amplitude = 0.06f;
    public float speed = 3f;

    private Vector3 baseScale;
    private float phase;

    private void Awake()
    {
        baseScale = transform.localScale;
        phase = Random.Range(0f, 6.28f);
    }

    private void Update()
    {
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        phase += delta * speed;
        float safeAmplitude = Mathf.Clamp(amplitude, 0f, 0.08f);
        transform.localScale = baseScale * (1f + Mathf.Sin(phase) * safeAmplitude);
    }
}
