using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 offset;
    private Vector2 localMousePosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Convert the mouse position to local point in the canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localMousePosition
        );

        // Calculate the offset: difference between the UI element's anchored position and the mouse position
        offset = rectTransform.anchoredPosition - localMousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null || canvas == null) return;

        // Convert the current mouse position to local point in the canvas space
        Vector2 currentMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out currentMousePosition
        );

        // Update the position, considering the offset
        rectTransform.anchoredPosition = currentMousePosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Optional: Actions to take when drag ends
    }
}
