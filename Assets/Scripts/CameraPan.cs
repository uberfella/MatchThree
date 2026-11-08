using UnityEngine;
using UnityEngine.EventSystems;

// Drag to pan an orthographic camera.
// PC: right/middle mouse drag anywhere, or left drag on empty space (left drag on a cell swaps it).
// Mobile: one finger on empty space, or two fingers anywhere.
[RequireComponent(typeof(Camera))]
public class CameraPan : MonoBehaviour
{
    [SerializeField] private bool clampToBounds = false;
    [SerializeField] private Vector2 minPosition = new Vector2(-20f, -20f);
    [SerializeField] private Vector2 maxPosition = new Vector2(20f, 20f);

    private Camera cam;
    private bool panning;
    private Vector3 lastWorldPoint;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (Input.touchCount >= 2) HandleTwoFingers();
        else if (Input.touchCount == 1) HandlePointer(Input.GetTouch(0).position, Input.GetTouch(0).phase, false);
        else HandleMouse();
    }

    private void HandleMouse()
    {
        bool altButton = Input.GetMouseButton(1) || Input.GetMouseButton(2);
        bool altDown = Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);

        if (altDown) BeginPan(Input.mousePosition);
        else if (Input.GetMouseButtonDown(0) && !IsOverCollider(Input.mousePosition)) BeginPan(Input.mousePosition);

        if (panning)
        {
            if (Input.GetMouseButton(0) || altButton) ContinuePan(Input.mousePosition);
            else panning = false;
        }
    }

    private void HandlePointer(Vector2 pos, TouchPhase phase, bool force)
    {
        if (phase == TouchPhase.Began)
        {
            if (force || !IsOverCollider(pos)) BeginPan(pos);
        }
        else if (phase == TouchPhase.Moved || phase == TouchPhase.Stationary)
        {
            if (panning) ContinuePan(pos);
        }
        else panning = false;
    }

    private void HandleTwoFingers()
    {
        Vector2 center = (Input.GetTouch(0).position + Input.GetTouch(1).position) * 0.5f;
        if (!panning) BeginPan(center);
        else ContinuePan(center);
    }

    private void BeginPan(Vector2 screenPos)
    {
        panning = true;
        lastWorldPoint = cam.ScreenToWorldPoint(screenPos);
    }

    private void ContinuePan(Vector2 screenPos)
    {
        Vector3 current = cam.ScreenToWorldPoint(screenPos);
        Vector3 pos = transform.position + (lastWorldPoint - current);

        if (clampToBounds)
        {
            pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
        }

        transform.position = pos;
        // Camera moved, so re-sample under the same screen point to avoid drift
        lastWorldPoint = cam.ScreenToWorldPoint(screenPos);
    }

    private bool IsOverCollider(Vector2 screenPos)
    {
        if (EventSystem.current != null && (EventSystem.current.IsPointerOverGameObject()
            || (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))) return true;
        Vector2 world = cam.ScreenToWorldPoint(screenPos);
        return Physics2D.OverlapPoint(world) != null;
    }
}
