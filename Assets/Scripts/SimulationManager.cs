using System;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public event Action OnTimeScaleChanged;

    [SerializeField] private float timeScale = 1f;

    public float TimeScale
    {
        get { return timeScale; }
    }

    public float ScaledDeltaTime
    {
        get { return Time.deltaTime * timeScale; }
    }

    public void SetTimeScale(float scale)
    {
        timeScale = Mathf.Max(0f, scale);
        if (OnTimeScaleChanged != null)
        {
            OnTimeScaleChanged.Invoke();
        }
    }
}
