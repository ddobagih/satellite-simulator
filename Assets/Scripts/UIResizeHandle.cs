using UnityEngine;
using UnityEngine.EventSystems;

public class UIResizeHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform Target;
    public Vector2 MinSize = new Vector2(260f, 180f);
    public Vector2 MaxSize = new Vector2(1120f, 760f);
    public string PersistKey;

    private Canvas rootCanvas;
    private Vector2 startSize;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Target == null)
        {
            Target = transform.parent as RectTransform;
        }
        if (rootCanvas == null)
        {
            rootCanvas = GetComponentInParent<Canvas>();
        }
        if (Target != null)
        {
            Target.SetAsLastSibling();
            startSize = Target.rect.size;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Target == null)
        {
            return;
        }

        float scale = rootCanvas != null ? Mathf.Max(rootCanvas.scaleFactor, 0.01f) : 1f;
        Vector2 delta = eventData.delta / scale;
        Vector2 size = Target.rect.size + new Vector2(delta.x, -delta.y);
        size.x = Mathf.Clamp(size.x, MinSize.x, Mathf.Max(MinSize.x, MaxSize.x));
        size.y = Mathf.Clamp(size.y, MinSize.y, Mathf.Max(MinSize.y, MaxSize.y));
        Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        Target.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        ClampTargetToSafeArea();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Target == null || string.IsNullOrEmpty(PersistKey))
        {
            return;
        }

        ClampTargetToSafeArea();
        Vector2 size = Target.rect.size;
        PlayerPrefs.SetFloat(PersistKey + ".w", size.x);
        PlayerPrefs.SetFloat(PersistKey + ".h", size.y);
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
