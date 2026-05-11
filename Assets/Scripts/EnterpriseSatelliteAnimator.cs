using UnityEngine;

public class EnterpriseSatelliteAnimator : MonoBehaviour
{
    public SimulationManager simulationManager;
    public float radius = 6.9f;
    public float speed = 8f;
    public float inclination = -18f;
    public float angle = 65f;

    private void Update()
    {
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        angle = Mathf.Repeat(angle + speed * delta, 360f);
        float radians = angle * Mathf.Deg2Rad;
        Vector3 local = new Vector3(Mathf.Cos(radians) * radius, 0f, Mathf.Sin(radians) * radius);
        transform.position = Quaternion.Euler(inclination, 0f, 0f) * local;
        transform.LookAt(Vector3.zero);
    }
}
