using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("References")]
    public RectTransform background;    // assign JoystickBG (this GameObject's RectTransform)
    public RectTransform handle;        // assign JoystickHandle

    [Header("Settings")]
    public float handleRange = 100f;    // how far (in px) the handle can move from center

    private Vector2 input = Vector2.zero;  // the normalized input value

    // Called when pointer first touches the joystick background
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // Called while dragging
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, background.position);
        Vector2 radius = background.sizeDelta / 2f;
        Vector2 pointerDelta = eventData.position - position;

        // Normalize pointer delta relative to radius
        Vector2 normalized = new Vector2(
            Mathf.Clamp(pointerDelta.x / radius.x, -1f, 1f),
            Mathf.Clamp(pointerDelta.y / radius.y, -1f, 1f)
        );

        input = normalized.magnitude > 1f
            ? normalized.normalized
            : normalized;

        // Move the handle
        handle.anchoredPosition = new Vector2(input.x * radius.x, input.y * radius.y);
    }

    // Called when pointer is lifted
    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }

    /// <summary>
    /// Returns horizontal input in range –1…+1.
    /// For a 2D platformer you can just read this property.
    /// </summary>
    public float Horizontal => input.x;

    /// <summary>
    /// Returns vertical input (–1…+1), if you need it.
    /// </summary>
    public float Vertical => input.y;
}
