using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public float TimeScale
    {
        get { return 1f; }
    }

    public float ScaledDeltaTime
    {
        get { return Time.deltaTime; }
    }
}
