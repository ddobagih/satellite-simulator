using System.Collections.Generic;
using UnityEngine;

public class SatelliteFocusController : MonoBehaviour
{
    public SatelliteManager satelliteManager;
    public CameraManager cameraManager;

    private readonly Dictionary<string, Transform> satelliteTransforms = new Dictionary<string, Transform>();
    private readonly Dictionary<string, GameObject> focusHalos = new Dictionary<string, GameObject>();
    private Material focusMaterial;

    public void Initialize(SatelliteManager manager, CameraManager cameras, Material haloMaterial)
    {
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged -= HandleSelectionChanged;
        }

        satelliteManager = manager;
        cameraManager = cameras;
        focusMaterial = haloMaterial;
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged += HandleSelectionChanged;
        }
    }

    private void OnDestroy()
    {
        if (satelliteManager != null)
        {
            satelliteManager.OnSelectionChanged -= HandleSelectionChanged;
        }
    }

    public void RegisterSatellite(string satelliteName, Transform satelliteTransform)
    {
        satelliteTransforms[satelliteName] = satelliteTransform;
        GameObject halo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        halo.name = satelliteName + " Focus Halo";
        halo.transform.SetParent(satelliteTransform, false);
        halo.transform.localPosition = Vector3.zero;
        halo.transform.localScale = Vector3.one * 1.15f;
        halo.GetComponent<Renderer>().material = focusMaterial;
        Destroy(halo.GetComponent<Collider>());
        halo.SetActive(satelliteManager != null
            && satelliteManager.SelectedSatellite != null
            && satelliteManager.SelectedSatellite.satelliteName == satelliteName);
        focusHalos[satelliteName] = halo;
    }

    private void HandleSelectionChanged(SatelliteData selected)
    {
        foreach (KeyValuePair<string, GameObject> pair in focusHalos)
        {
            pair.Value.SetActive(selected != null && pair.Key == selected.satelliteName);
        }

        if (selected != null && cameraManager != null && satelliteTransforms.ContainsKey(selected.satelliteName))
        {
            cameraManager.satellite = satelliteTransforms[selected.satelliteName];
        }
    }
}
