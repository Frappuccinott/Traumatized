using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Image cursorImage;
    [SerializeField] private float cursorSpeed = 1000f;

    private Canvas canvas;
    private Vector2 cursorPosition;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);

        if (cursorTransform != null)
        {
            cursorTransform.gameObject.SetActive(true);
        }

        // Sistem cursor'unu gizle
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateCursorPosition();
    }

    private void Update()
    {
        UpdateGamepadCursor();
    }

    private void UpdateGamepadCursor()
    {
        if (Gamepad.current == null) return;

        // Right Stick ile hareket
        Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
        cursorPosition += rightStick * cursorSpeed * Time.deltaTime;

        // Ekran sýnýrlarý
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);

        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        if (cursorTransform != null && canvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                cursorPosition,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            cursorTransform.localPosition = localPoint;
        }
    }

    public Vector2 GetCursorPosition() => cursorPosition;
}