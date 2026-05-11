using UnityEngine;

public class SatelliteOrbit : MonoBehaviour
{
    public Transform earth;
    public SimulationManager simulationManager;
    public float orbitRadius = 7.2f;
    public float orbitSpeed = 18f;
    public float orbitInclination = 25f;
    public float initialAngle = 215f;

    private float angle;

    public float CurrentAngle
    {
        get { return angle; }
    }

    private void Start()
    {
        angle = initialAngle;
        ApplyPosition();
    }

    private void Update()
    {
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        angle = Mathf.Repeat(angle + orbitSpeed * delta, 360f);
        ApplyPosition();
    }

    public Vector3 GetOrbitPosition(float degrees)
    {
        Vector3 center = earth != null ? earth.position : Vector3.zero;
        float radians = degrees * Mathf.Deg2Rad;
        Vector3 planePosition = new Vector3(Mathf.Cos(radians) * orbitRadius, 0f, Mathf.Sin(radians) * orbitRadius);
        Quaternion tilt = Quaternion.Euler(orbitInclination, 0f, 0f);
        return center + tilt * planePosition;
    }

    public void SetAngle(float degrees)
    {
        angle = Mathf.Repeat(degrees, 360f);
        ApplyPosition();
    }

    private void ApplyPosition()
    {
        transform.position = GetOrbitPosition(angle);
        Vector3 center = earth != null ? earth.position : Vector3.zero;
        Vector3 tangent = GetOrbitPosition(angle + 1f) - transform.position;
        if (tangent.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(tangent.normalized, (transform.position - center).normalized);
        }
    }
}
