using UnityEngine;

public class AssetStoreIntegrationGuide : MonoBehaviour
{
    [Header("Optional imported Asset Store prefabs")]
    public GameObject satelliteModelPrefab;
    public Material premiumEarthMaterial;
    public Material spaceSkybox;
    public AudioClip reservationSfx;
    public AudioClip missionCompleteSfx;

    [TextArea(4, 8)]
    public string importNotes =
        "Asset Store assets cannot be imported without Unity Editor account access. " +
        "After importing a satellite model, Earth material, skybox, or SFX pack, assign them here " +
        "and replace the procedural visuals in OrbitShareBootstrap if higher fidelity is needed.";
}
