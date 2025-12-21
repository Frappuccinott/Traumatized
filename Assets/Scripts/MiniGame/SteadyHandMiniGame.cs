using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Steady Hand - Success → Panel kapat, Fail → Red screen + reload
/// </summary>
public class SteadyHandMiniGame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform dot;
    [SerializeField] private RectTransform limitCircle;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 5f;

    [Header("Vibration & Noise")]
    [Range(0f, 1f)]
    [SerializeField] private float vibrationStrength = 1.0f;
    [SerializeField] private float noiseStrength = 400f;

    [Header("Player Control")]
    [SerializeField] private float stickControlSpeed = 250f;

    public event Action OnSuccess;

    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 offset;
    private float timer;
    private bool isPlaying;
    private Gamepad gamepad;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void StartGame()
    {
        Time.timeScale = 0f;

        offset = Vector2.zero;
        timer = 0f;
        isPlaying = true;
        moveInput = Vector2.zero;

        if (dot != null)
        {
            dot.anchoredPosition = Vector2.zero;
        }

        gamepad = Gamepad.current;

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(vibrationStrength, vibrationStrength);
        }
    }

    public void StopGame()
    {
        isPlaying = false;
        ResetGameState();
    }

    private void Update()
    {
        if (!isPlaying) return;

        float deltaTime = Time.unscaledDeltaTime;
        timer += deltaTime;

        if (timer >= gameDuration)
        {
            Success();
            return;
        }

        UpdateDotPosition(deltaTime);
        CheckFail();
    }

    private void UpdateDotPosition(float deltaTime)
    {
        Vector2 pushDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Vector2 pushForce = pushDirection * noiseStrength * deltaTime;
        offset += pushForce;

        Vector2 playerControl = new Vector2(-moveInput.x, moveInput.y) * stickControlSpeed * deltaTime;
        offset += playerControl;

        if (dot != null)
        {
            dot.anchoredPosition = offset;
        }
    }

    private void CheckFail()
    {
        if (limitCircle == null || dot == null) return;

        float circleRadius = limitCircle.rect.width * 0.5f;
        float dotRadius = dot.rect.width * 0.5f;
        float distance = offset.magnitude;

        if (distance + dotRadius >= circleRadius)
        {
            Fail();
        }
    }

    private void Success()
    {
        isPlaying = false;
        ResetGameState();
        OnSuccess?.Invoke();
    }

    private void Fail()
    {
        isPlaying = false;
        StopVibration();

        Time.timeScale = 1f;

        ShowRedScreenAndReload();
    }

    private void ShowRedScreenAndReload()
    {
        RedScreenEffect.Instance?.ShowRedScreen();
        Invoke(nameof(ReloadScene), 1f); // 1 saniye sonra reload
    }

    private void ResetGameState()
    {
        StopVibration();
        Time.timeScale = 1f;
    }

    private void StopVibration()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
            gamepad.ResetHaptics();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        inputActions?.Dispose();
    }
}