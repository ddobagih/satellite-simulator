using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform Target;
    public string PersistKey;

    private Canvas rootCanvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Target == null)
        {
            Target = transform as RectTransform;
        }

        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
        }

        if (Target != null)
        {
            Target.SetAsLastSibling();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Target == null)
        {
            return;
        }

        float scale = rootCanvas != null ? Mathf.Max(rootCanvas.scaleFactor, 0.01f) : 1f;
        Target.anchoredPosition += eventData.delta / scale;
        ClampTargetToSafeArea();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Target == null || string.IsNullOrEmpty(PersistKey))
        {
            return;
        }

        ClampTargetToSafeArea();
        PlayerPrefs.SetFloat(PersistKey + ".x", Target.anchoredPosition.x);
        PlayerPrefs.SetFloat(PersistKey + ".y", Target.anchoredPosition.y);
        PlayerPrefs.Save();
    }

    private void ClampTargetToSafeArea()
    {
        if (Target == null)
        {
            return;
        }
        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
        }

        RectTransform canvasRect = rootCanvas != null ? rootCanvas.transform as RectTransform : null;
        RectTransform parentRect = Target.parent as RectTransform;
        if (canvasRect == null || parentRect == null)
        {
            return;
        }

        Vector3[] worldCorners = new Vector3[4];
        Target.GetWorldCorners(worldCorners);
        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);
        for (int i = 0; i < worldCorners.Length; i++)
        {
            Vector3 local = canvasRect.InverseTransformPoint(worldCorners[i]);
            min = Vector2.Min(min, local);
            max = Vector2.Max(max, local);
        }

        Rect canvas = canvasRect.rect;
        float safeMinX = canvas.xMin + canvas.width * 0.01f;
        float safeMaxX = canvas.xMax - canvas.width * 0.01f;
        float safeMinY = canvas.yMin + canvas.height * 0.075f;
        float safeMaxY = canvas.yMax - canvas.height * 0.105f;
        Vector2 canvasDelta = Vector2.zero;
        if (min.x < safeMinX)
        {
            canvasDelta.x = safeMinX - min.x;
        }
        else if (max.x > safeMaxX)
        {
            canvasDelta.x = safeMaxX - max.x;
        }
        if (min.y < safeMinY)
        {
            canvasDelta.y = safeMinY - min.y;
        }
        else if (max.y > safeMaxY)
        {
            canvasDelta.y = safeMaxY - max.y;
        }

        if (canvasDelta.sqrMagnitude < 0.001f)
        {
            return;
        }

        Vector3 worldDelta = canvasRect.TransformVector(canvasDelta);
        Vector3 parentDelta = parentRect.InverseTransformVector(worldDelta);
        Target.anchoredPosition += new Vector2(parentDelta.x, parentDelta.y);
    }
}
