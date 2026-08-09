using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("Joystick Components")]
    public RectTransform background;
    public RectTransform handle;
    
    [Header("Settings")]
    public float handleRange = 1f;
    public float deadZone = 0.1f;
    
    private Vector2 inputVector;
    private Vector2 joystickCenter;
    
    public float Horizontal { get { return inputVector.x; } }
    public float Vertical { get { return inputVector.y; } }
    public Vector2 Direction { get { return inputVector; } }
    
    void Start()
    {
        // Auto-find components if not assigned
        if (background == null)
            background = GetComponent<RectTransform>();
            
        if (handle == null && transform.childCount > 0)
            handle = transform.GetChild(0).GetComponent<RectTransform>();
        
        // Calculate center
        joystickCenter = background.position;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos))
        {
            // Calculate input
            pos.x /= background.sizeDelta.x;
            pos.y /= background.sizeDelta.y;
            
            inputVector = new Vector2(pos.x * 2, pos.y * 2);
            
            // Apply dead zone
            if (inputVector.magnitude < deadZone)
            {
                inputVector = Vector2.zero;
            }
            else if (inputVector.magnitude > 1)
            {
                inputVector = inputVector.normalized;
            }
            
            // Move handle
            if (handle != null)
            {
                handle.anchoredPosition = new Vector2(
                    inputVector.x * background.sizeDelta.x / 2 * handleRange,
                    inputVector.y * background.sizeDelta.y / 2 * handleRange
                );
            }
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset
        inputVector = Vector2.zero;
        
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
    
    // Get direction with optional smoothing
    public Vector2 GetDirection(float smoothing = 0f)
    {
        if (smoothing > 0)
        {
            return Vector2.Lerp(Vector2.zero, inputVector, smoothing);
        }
        return inputVector;
    }
}