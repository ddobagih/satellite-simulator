using UnityEngine;

public class BillboardLabel : MonoBehaviour
{
    public bool autoVisibility = true;
    public float fullAlphaDistance = 8f;
    public float hiddenDistance = 18f;

    private Camera targetCamera;
    private TextMesh textMesh;
    private Vector3 baseScale;
    private float baseAlpha = 1f;
    private bool initialized;
    private bool hasVisibilityOverride;
    private bool forceVisibleOverride;
    private float alphaOverride = 1f;
    private float scaleOverride = 1f;

    private void LateUpdate()
    {
        EnsureInitialized();

        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(transform.position - targetCamera.transform.position);
        UpdateVisibility();
    }

    public void SetVisibilityOverride(float alpha, float scaleMultiplier, bool forceVisible)
    {
        hasVisibilityOverride = true;
        alphaOverride = Mathf.Clamp01(alpha);
        scaleOverride = Mathf.Clamp(scaleMultiplier, 0.45f, 1.35f);
        forceVisibleOverride = forceVisible;
    }

    private void EnsureInitialized()
    {
        if (initialized)
        {
            return;
        }

        initialized = true;
        textMesh = GetComponent<TextMesh>();
        baseScale = transform.localScale;
        if (textMesh != null)
        {
            baseAlpha = textMesh.color.a;
        }
    }

    private void UpdateVisibility()
    {
        if (textMesh == null)
        {
            return;
        }

        float autoAlpha = autoVisibility ? CalculateAutoAlpha() : 1f;
        bool canForceVisible = forceVisibleOverride && targetCamera.WorldToViewportPoint(transform.position).z > 0f;
        float alpha = baseAlpha * autoAlpha;
        float scale = Mathf.Lerp(0.72f, 1f, autoAlpha);

        if (hasVisibilityOverride)
        {
            alpha = autoVisibility ? alpha * alphaOverride : alphaOverride;
            scale *= scaleOverride;
            if (canForceVisible)
            {
                alpha = Mathf.Max(alpha, alphaOverride * 0.85f);
                scale = Mathf.Max(scale, 1f * scaleOverride);
            }
        }

        Color color = textMesh.color;
        color.a = Mathf.Clamp01(alpha);
        textMesh.color = color;
        transform.localScale = baseScale * Mathf.Clamp(scale, 0.5f, 1.35f);
    }

    private float CalculateAutoAlpha()
    {
        Vector3 viewport = targetCamera.WorldToViewportPoint(transform.position);
        if (viewport.z <= 0f)
        {
            return 0f;
        }

        float edgeFade = Mathf.Min(
            Mathf.Min(Mathf.InverseLerp(0f, 0.06f, viewport.x), Mathf.InverseLerp(1f, 0.94f, viewport.x)),
            Mathf.Min(Mathf.InverseLerp(0f, 0.08f, viewport.y), Mathf.InverseLerp(1f, 0.92f, viewport.y)));
        float distance = Vector3.Distance(targetCamera.transform.position, transform.position);
        float distanceFade = Mathf.InverseLerp(hiddenDistance, fullAlphaDistance, distance);
        float sideFade = 1f;
        Vector3 fromCenter = transform.position;
        if (fromCenter.magnitude < 5.2f)
        {
            Vector3 cameraDirection = targetCamera.transform.position.sqrMagnitude > 0.001f
                ? targetCamera.transform.position.normalized
                : Vector3.forward;
            sideFade = Mathf.InverseLerp(-0.08f, 0.22f, Vector3.Dot(fromCenter.normalized, cameraDirection));
        }

        return Mathf.Clamp01(edgeFade * distanceFade * sideFade);
    }
}
