using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VirtualCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;

    [Header("Input")]
    [SerializeField] private InputActionReference lookAction;

    [Header("Sensitivity")]
    [SerializeField] private float gamepadSensitivity = 900f;
    [SerializeField] private float mouseSensitivity = 1.0f;

    private Vector2 cursorPosition;

    private void OnEnable() => lookAction?.action.Enable();

    private void OnDisable() => lookAction?.action.Disable();

    private void Start()
    {
        cursorPosition = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        UpdateCursorVisual();
    }

    private void Update() => UpdateCursorInput();

    private void UpdateCursorInput()
    {
        if (lookAction == null) return;

        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();
        bool usingMouse = Mouse.current != null && Mouse.current.delta.ReadValue() != Vector2.zero;

        if (usingMouse)
            cursorPosition += lookInput * mouseSensitivity;
        else
            cursorPosition += lookInput * gamepadSensitivity * Time.deltaTime;

        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0f, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0f, Screen.height);

        UpdateCursorVisual();
    }

    private void UpdateCursorVisual()
    {
        if (cursorTransform == null || canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            cursorPosition,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPoint
        );

        cursorTransform.localPosition = localPoint;
    }

    public Vector2 GetCursorScreenPosition() => cursorPosition;

    public Vector2 GetCursorPosition() => cursorPosition;
}