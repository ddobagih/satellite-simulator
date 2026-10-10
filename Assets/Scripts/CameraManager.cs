using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public Camera targetCamera;
    public Transform earth;
    public Transform satellite;
    public Transform missionTarget;
    public Transform groundStation;
    public bool protectSmallScreenInput = true;
    public int smallScreenMaxWidth = 960;
    public int smallScreenMaxHeight = 720;
    public float smallScreenLeftUiPixels = 220f;
    public float smallScreenRightUiPixels = 210f;
    public float smallScreenTopUiPixels = 82f;
    public float smallScreenBottomUiPixels = 72f;
    public bool allowRightMouseOrbit = true;
    public bool allowLeftMouseEarthOrbit = true;
    public bool allowKeyboardOrbit = true;
    public float keyboardOrbitDegreesPerSecond = 62f;
    public float keyboardZoomSpeed = 3.2f;

    private string mode = "Overview";
    private Vector3 desiredPosition;
    private Vector3 desiredLookAt;
    private Vector3 framingOffset;
    private bool useSmoothMove;
    private float zoomOffset;
    private float desiredZoomOffset;
    private float zoomVelocity;
    private bool isDraggingEarth;
    private bool pendingEarthDrag;
    private Vector3 dragStartMousePosition;
    private Vector3 lastMousePosition;
    private Vector2 orbitAngularVelocity;
    private int activeOrbitMouseButton = -1;
    private readonly List<RaycastResult> uiRaycastResults = new List<RaycastResult>();
    private const float MinZoomDistance = 4.4f;
    private const float MaxZoomDistance = 18f;
    private const float ZoomSensitivity = 0.9f;
    private const float ZoomSmoothTime = 0.12f;
    private const float PointerZoomPanLimit = 2.4f;
    private const float PointerZoomPanStepLimit = 0.55f;
    private const float DragSensitivity = 0.13f;
    private const float DragStartThresholdPixels = 5f;
    private const float DragInertiaDamping = 6.4f;
    private const float MaxDragInertia = 82f;
    private const float DestinationClickGuardDistance = 0.25f;
    private const float MinOrbitPitch = 18f;
    private const float MaxOrbitPitch = 162f;

    public string CurrentMode
    {
        get { return mode; }
    }

    public bool OrbitControlsEnabled
    {
        get { return allowRightMouseOrbit || allowLeftMouseEarthOrbit || allowKeyboardOrbit; }
    }

    public string OrbitControlsSummary
    {
        get { return "우클릭 드래그/지구 좌클릭 드래그: 회전 · 휠/QE: 줌 · R: 초기화"; }
    }

    private void Start()
    {
        if (targetCamera != null)
        {
            desiredPosition = targetCamera.transform.position;
            desiredLookAt = earth != null ? earth.position : Vector3.zero;
            desiredZoomOffset = zoomOffset;
        }
    }

    private void Update()
    {
        if (targetCamera == null)
        {
            return;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f && !IsPointerInputBlocked())
        {
            HandlePointerZoom(scroll);
        }

        UpdateEarthDrag();
        UpdateKeyboardOrbit();
        UpdateOrbitInertia();
    }

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            return;
        }

        zoomOffset = Mathf.SmoothDamp(zoomOffset, desiredZoomOffset, ref zoomVelocity, ZoomSmoothTime);

        if (mode == "Satellite" && satellite != null)
        {
            desiredPosition = satellite.position + new Vector3(-6.8f, 4.4f, -6.8f) + framingOffset;
            desiredLookAt = satellite.position + framingOffset;
            useSmoothMove = true;
        }
        else if (mode == "Mission" && missionTarget != null)
        {
            Vector3 station = groundStation != null ? groundStation.position : Vector3.zero;
            Vector3 center = (missionTarget.position + station) * 0.5f;
            desiredPosition = center + new Vector3(7.4f, 4.9f, -9.0f) + framingOffset;
            desiredLookAt = center + framingOffset;
            useSmoothMove = true;
        }

        if (useSmoothMove)
        {
            Vector3 zoomedPosition = ApplyZoom(desiredPosition, desiredLookAt);
            targetCamera.transform.position = Vector3.Lerp(targetCamera.transform.position, zoomedPosition, Time.deltaTime * 4.5f);
            Vector3 lookDirection = desiredLookAt - targetCamera.transform.position;
            if (lookDirection.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
                targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, desiredRotation, Time.deltaTime * 5.5f);
            }
        }
    }

    public void SetOverview()
    {
        mode = "Overview";
        framingOffset = Vector3.zero;
        orbitAngularVelocity = Vector2.zero;
        MoveTo(new Vector3(0f, 6.7f, -13.2f), Vector3.zero, true);
    }

    public void SetEarth()
    {
        mode = "Earth";
        framingOffset = Vector3.zero;
        orbitAngularVelocity = Vector2.zero;
        MoveTo(new Vector3(0f, 3.9f, -8.8f), Vector3.zero, true);
    }

    public void ResetEarthOrbit()
    {
        desiredZoomOffset = 0f;
        zoomOffset = 0f;
        zoomVelocity = 0f;
        SetEarth();
    }

    public void SetSatellite()
    {
        mode = "Satellite";
        framingOffset = Vector3.zero;
        orbitAngularVelocity = Vector2.zero;
    }

    public void SetMission()
    {
        mode = "Mission";
        framingOffset = Vector3.zero;
        orbitAngularVelocity = Vector2.zero;
    }

    public bool IsPointerInputBlocked()
    {
        return IsPointerBlockedByEventSystem() || IsPointerInsideSmallScreenUiGuard();
    }

    private void MoveTo(Vector3 position, Vector3 lookAt, bool smooth)
    {
        if (targetCamera == null)
        {
            return;
        }

        desiredPosition = position;
        desiredLookAt = lookAt;
        useSmoothMove = smooth;
        if (!smooth)
        {
            targetCamera.transform.position = ApplyZoom(position, lookAt);
            targetCamera.transform.LookAt(lookAt);
        }
    }

    private Vector3 ApplyZoom(Vector3 basePosition, Vector3 lookAt)
    {
        return ApplyZoomWithOffset(basePosition, lookAt, zoomOffset);
    }

    private Vector3 ApplyZoomWithOffset(Vector3 basePosition, Vector3 lookAt, float offset)
    {
        Vector3 direction = basePosition - lookAt;
        float distance = direction.magnitude;
        if (distance < 0.01f)
        {
            return basePosition;
        }

        float zoomedDistance = Mathf.Clamp(distance + offset, MinZoomDistance, MaxZoomDistance);
        return lookAt + direction.normalized * zoomedDistance;
    }

    private void HandlePointerZoom(float scroll)
    {
        Ray pointerRay = targetCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 focusPoint;
        float focusDistance;
        bool hasFocusPoint = TryGetPointerFocusPoint(pointerRay, out focusPoint, out focusDistance);

        float previousZoomOffset = desiredZoomOffset;
        desiredZoomOffset = ClampZoomOffset(desiredZoomOffset - scroll * ZoomSensitivity);
        if (hasFocusPoint && !Mathf.Approximately(previousZoomOffset, desiredZoomOffset))
        {
            ApplyPointerZoomPan(pointerRay, previousZoomOffset);
        }

        useSmoothMove = true;
    }

    private float ClampZoomOffset(float offset)
    {
        float baseDistance = Vector3.Distance(desiredPosition, desiredLookAt);
        if (baseDistance < 0.01f)
        {
            return offset;
        }

        return Mathf.Clamp(offset, MinZoomDistance - baseDistance, MaxZoomDistance - baseDistance);
    }

    private bool TryGetPointerFocusPoint(Ray pointerRay, out Vector3 focusPoint, out float focusDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(pointerRay, out hit, 1000f))
        {
            focusPoint = hit.point;
            focusDistance = Mathf.Max(0.1f, hit.distance);
            return true;
        }

        Plane focusPlane = new Plane(targetCamera.transform.forward, desiredLookAt);
        float enter;
        if (focusPlane.Raycast(pointerRay, out enter) && enter > 0.1f)
        {
            focusPoint = pointerRay.GetPoint(enter);
            focusDistance = enter;
            return true;
        }

        focusDistance = Mathf.Max(1f, Vector3.Distance(targetCamera.transform.position, desiredLookAt));
        focusPoint = pointerRay.GetPoint(focusDistance);
        return true;
    }

    private void ApplyPointerZoomPan(Ray pointerRay, float previousZoomOffset)
    {
        Vector3 previousZoomedPosition = ApplyZoomWithOffset(desiredPosition, desiredLookAt, previousZoomOffset);
        Vector3 zoomedPosition = ApplyZoomWithOffset(desiredPosition, desiredLookAt, desiredZoomOffset);
        Vector3 centerMove = zoomedPosition - previousZoomedPosition;
        float pointerMove = Vector3.Dot(centerMove, targetCamera.transform.forward);
        Vector3 pan = previousZoomedPosition + pointerRay.direction.normalized * pointerMove - zoomedPosition;
        pan = Vector3.ProjectOnPlane(pan, targetCamera.transform.forward);
        if (pan.sqrMagnitude > PointerZoomPanStepLimit * PointerZoomPanStepLimit)
        {
            pan = pan.normalized * PointerZoomPanStepLimit;
        }

        Vector3 nextFramingOffset = framingOffset + pan;
        if (nextFramingOffset.sqrMagnitude > PointerZoomPanLimit * PointerZoomPanLimit)
        {
            nextFramingOffset = nextFramingOffset.normalized * PointerZoomPanLimit;
        }
        Vector3 appliedPan = nextFramingOffset - framingOffset;
        desiredPosition += appliedPan;
        desiredLookAt += appliedPan;
        framingOffset = nextFramingOffset;
    }

    private void UpdateEarthDrag()
    {
        if (earth == null || targetCamera == null)
        {
            return;
        }

        if (activeOrbitMouseButton >= 0 && Input.GetMouseButtonUp(activeOrbitMouseButton))
        {
            EndOrbitDrag();
        }

        if (activeOrbitMouseButton < 0)
        {
            if (allowRightMouseOrbit && Input.GetMouseButtonDown(1))
            {
                BeginPendingOrbitDrag(1, false);
            }
            else if (allowLeftMouseEarthOrbit && Input.GetMouseButtonDown(0))
            {
                BeginPendingOrbitDrag(0, true);
            }
        }

        if (activeOrbitMouseButton < 0 || !Input.GetMouseButton(activeOrbitMouseButton))
        {
            return;
        }

        if (!pendingEarthDrag && !isDraggingEarth)
        {
            return;
        }

        Vector3 delta = Input.mousePosition - lastMousePosition;
        if (!isDraggingEarth)
        {
            Vector3 dragDelta = Input.mousePosition - dragStartMousePosition;
            if (dragDelta.sqrMagnitude < DragStartThresholdPixels * DragStartThresholdPixels)
            {
                orbitAngularVelocity = Vector2.Lerp(orbitAngularVelocity, Vector2.zero, Time.deltaTime * DragInertiaDamping);
                return;
            }
            BeginEarthDrag();
            delta = Input.mousePosition - lastMousePosition;
        }

        lastMousePosition = Input.mousePosition;
        if (delta.sqrMagnitude < 0.01f)
        {
            orbitAngularVelocity = Vector2.Lerp(orbitAngularVelocity, Vector2.zero, Time.deltaTime * DragInertiaDamping);
            return;
        }

        float yawDegrees = delta.x * DragSensitivity;
        float pitchDegrees = -delta.y * DragSensitivity;
        ApplyOrbitDelta(yawDegrees, pitchDegrees, true);

        float safeDeltaTime = Mathf.Max(Time.deltaTime, 0.001f);
        orbitAngularVelocity = Vector2.ClampMagnitude(new Vector2(yawDegrees, pitchDegrees) / safeDeltaTime, MaxDragInertia);
    }

    private void BeginPendingOrbitDrag(int mouseButton, bool requireEarthHit)
    {
        pendingEarthDrag = CanStartOrbitDrag(requireEarthHit);
        isDraggingEarth = false;
        activeOrbitMouseButton = pendingEarthDrag ? mouseButton : -1;
        dragStartMousePosition = Input.mousePosition;
        lastMousePosition = Input.mousePosition;
        if (pendingEarthDrag)
        {
            orbitAngularVelocity = Vector2.zero;
        }
    }

    private void EndOrbitDrag()
    {
        pendingEarthDrag = false;
        isDraggingEarth = false;
        activeOrbitMouseButton = -1;
    }

    private void UpdateKeyboardOrbit()
    {
        if (!allowKeyboardOrbit || earth == null || targetCamera == null || IsKeyboardInputBlocked())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetEarthOrbit();
            return;
        }

        float yawInput = 0f;
        float pitchInput = 0f;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            yawInput -= 1f;
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            yawInput += 1f;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            pitchInput += 1f;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            pitchInput -= 1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            desiredZoomOffset = ClampZoomOffset(desiredZoomOffset + keyboardZoomSpeed * Time.deltaTime);
            useSmoothMove = true;
        }
        if (Input.GetKey(KeyCode.E))
        {
            desiredZoomOffset = ClampZoomOffset(desiredZoomOffset - keyboardZoomSpeed * Time.deltaTime);
            useSmoothMove = true;
        }

        if (Mathf.Abs(yawInput) < 0.01f && Mathf.Abs(pitchInput) < 0.01f)
        {
            return;
        }

        mode = "Earth";
        framingOffset = Vector3.zero;
        float step = keyboardOrbitDegreesPerSecond * Time.deltaTime;
        ApplyOrbitDelta(yawInput * step, pitchInput * step, false);
        orbitAngularVelocity = Vector2.zero;
    }

    private bool IsKeyboardInputBlocked()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
        {
            return false;
        }

        return EventSystem.current.currentSelectedGameObject.GetComponent<UnityEngine.UI.InputField>() != null;
    }

    public void OrbitEarthForTest(float yawDegrees, float pitchDegrees)
    {
        if (earth == null || targetCamera == null)
        {
            return;
        }

        mode = "Earth";
        framingOffset = Vector3.zero;
        desiredLookAt = earth.position;
        desiredPosition = desiredLookAt + CurrentUnzoomedCameraOffset(desiredLookAt);
        ApplyOrbitDelta(yawDegrees, pitchDegrees, true);
    }

    private void BeginEarthDrag()
    {
        pendingEarthDrag = false;
        isDraggingEarth = true;
        mode = "Earth";
        framingOffset = Vector3.zero;
        orbitAngularVelocity = Vector2.zero;
        desiredLookAt = earth.position;
        desiredPosition = desiredLookAt + CurrentUnzoomedCameraOffset(desiredLookAt);
    }

    private void UpdateOrbitInertia()
    {
        if (pendingEarthDrag || isDraggingEarth || mode != "Earth" || orbitAngularVelocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        float deltaTime = Time.deltaTime;
        ApplyOrbitDelta(orbitAngularVelocity.x * deltaTime, orbitAngularVelocity.y * deltaTime, false);
        orbitAngularVelocity = Vector2.Lerp(orbitAngularVelocity, Vector2.zero, deltaTime * DragInertiaDamping);
        useSmoothMove = true;
    }

    private void ApplyOrbitDelta(float yawDegrees, float pitchDegrees, bool snapCamera)
    {
        Vector3 pivot = earth.position;
        Vector3 offset = desiredPosition - pivot;
        if (offset.sqrMagnitude < 0.01f)
        {
            offset = CurrentUnzoomedCameraOffset(pivot);
        }

        Quaternion yaw = Quaternion.AngleAxis(yawDegrees, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(pitchDegrees, targetCamera.transform.right);
        Vector3 rotatedOffset = yaw * pitch * offset;
        rotatedOffset = ClampOrbitPitch(rotatedOffset, offset.magnitude);
        desiredPosition = pivot + rotatedOffset;
        desiredLookAt = pivot;
        useSmoothMove = true;

        if (snapCamera)
        {
            targetCamera.transform.position = ApplyZoom(desiredPosition, desiredLookAt);
            targetCamera.transform.LookAt(desiredLookAt);
        }
    }

    private bool CanStartOrbitDrag(bool requireEarthHit)
    {
        if (IsPointerInputBlocked())
        {
            return false;
        }

        if (!requireEarthHit)
        {
            return true;
        }

        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, 1000f))
        {
            return false;
        }

        if (hit.collider.GetComponentInParent<DestinationClickTarget>() != null
            || RaycastHitsDestinationClickTarget(ray, hit.distance + DestinationClickGuardDistance))
        {
            return false;
        }

        return hit.transform == earth || hit.transform.IsChildOf(earth) || IsNearEarthSurface(hit.point);
    }

    private bool RaycastHitsDestinationClickTarget(Ray ray, float maxDistance)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Max(0.01f, maxDistance));
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null && hits[i].collider.GetComponentInParent<DestinationClickTarget>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsNearEarthSurface(Vector3 worldPoint)
    {
        Vector3 scale = earth.lossyScale;
        float radius = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z)) * 0.5f;
        return (worldPoint - earth.position).sqrMagnitude <= radius * radius * 1.55f;
    }

    private Vector3 CurrentUnzoomedCameraOffset(Vector3 pivot)
    {
        Vector3 offset = targetCamera.transform.position - pivot;
        float distance = offset.magnitude;
        if (distance < 0.01f)
        {
            return new Vector3(0f, 3.4f, -7.5f);
        }

        float baseDistance = Mathf.Clamp(distance - zoomOffset, MinZoomDistance, MaxZoomDistance);
        return offset.normalized * baseDistance;
    }

    private Vector3 ClampOrbitPitch(Vector3 offset, float distance)
    {
        if (distance < 0.01f)
        {
            return offset;
        }

        float angleFromUp = Vector3.Angle(offset, Vector3.up);
        if (angleFromUp >= MinOrbitPitch && angleFromUp <= MaxOrbitPitch)
        {
            return offset;
        }

        Vector3 flat = Vector3.ProjectOnPlane(offset, Vector3.up);
        if (flat.sqrMagnitude < 0.0001f)
        {
            flat = Vector3.forward;
        }

        float clampedAngle = Mathf.Clamp(angleFromUp, MinOrbitPitch, MaxOrbitPitch) * Mathf.Deg2Rad;
        float y = Mathf.Cos(clampedAngle) * distance;
        float planar = Mathf.Sin(clampedAngle) * distance;
        return flat.normalized * planar + Vector3.up * y;
    }

    private bool IsPointerBlockedByEventSystem()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return true;
                }
            }
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        uiRaycastResults.Clear();
        EventSystem.current.RaycastAll(pointerData, uiRaycastResults);
        return uiRaycastResults.Count > 0;
    }

    private bool IsPointerInsideSmallScreenUiGuard()
    {
        if (!protectSmallScreenInput)
        {
            return false;
        }

        if (Screen.width > smallScreenMaxWidth && Screen.height > smallScreenMaxHeight)
        {
            return false;
        }

        Vector3 pointer = Input.mousePosition;
        if (pointer.x < 0f || pointer.y < 0f || pointer.x > Screen.width || pointer.y > Screen.height)
        {
            return true;
        }

        float left = Mathf.Min(smallScreenLeftUiPixels, Screen.width * 0.4f);
        float right = Mathf.Min(smallScreenRightUiPixels, Screen.width * 0.34f);
        float top = Mathf.Min(smallScreenTopUiPixels, Screen.height * 0.22f);
        float bottom = Mathf.Min(smallScreenBottomUiPixels, Screen.height * 0.2f);
        return pointer.x <= left
            || pointer.x >= Screen.width - right
            || pointer.y <= bottom
            || pointer.y >= Screen.height - top;
    }
}
