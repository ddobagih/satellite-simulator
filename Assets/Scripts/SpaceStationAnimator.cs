using UnityEngine;

public class SpaceStationAnimator : MonoBehaviour
{
    public SimulationManager simulationManager;
    public float orbitRadius = 8.65f;
    public float orbitSpeed = 4.5f;
    public float orbitInclination = -8f;
    public float angle = 300f;
    public Transform rotatingRing;

    private void Update()
    {
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        angle = Mathf.Repeat(angle + orbitSpeed * delta, 360f);
        float radians = angle * Mathf.Deg2Rad;
        Vector3 planePosition = new Vector3(Mathf.Cos(radians) * orbitRadius, 0f, Mathf.Sin(radians) * orbitRadius);
        transform.position = Quaternion.Euler(orbitInclination, 0f, 0f) * planePosition;
        transform.LookAt(Vector3.zero);

        if (rotatingRing != null)
        {
            rotatingRing.Rotate(Vector3.forward, 24f * delta, Space.Self);
        }
    }
}
