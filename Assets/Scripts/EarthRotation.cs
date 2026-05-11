using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    public SimulationManager simulationManager;
    public float rotationSpeed = 6f;

    private void Update()
    {
        float delta = simulationManager != null ? simulationManager.ScaledDeltaTime : Time.deltaTime;
        transform.Rotate(Vector3.up, rotationSpeed * delta, Space.World);
    }
}
