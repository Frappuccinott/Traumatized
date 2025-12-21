using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float noiseStrength = 400f; // ⬅️ ÇOK GÜÇLÜ

    [Header("Player Control")]
    [SerializeField] private float stickControlSpeed = 250f; // ⬅️ Güçlü kontrol gerekir

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

        Debug.Log("<color=cyan>[STEADY HAND] Dot çıkmaya çalışıyor - İçeride tut!</color>");
    }

    public void StopGame()
    {
        isPlaying = false;
        Time.timeScale = 1f;
        StopVibration();
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

        // 1. GÜÇLÜ RASTGELE İTME - Dot çıkmaya çalışır
        Vector2 pushDirection = UnityEngine.Random.insideUnitCircle.normalized; // Rastgele yön
        Vector2 pushForce = pushDirection * noiseStrength * deltaTime;
        offset += pushForce;

        // 2. PLAYER KONTROL - Dot'u geri çek
        Vector2 playerControl = new Vector2(-moveInput.x, moveInput.y) * stickControlSpeed * deltaTime;
        offset += playerControl;

        // Dot pozisyonu
        if (dot != null)
        {
            dot.anchoredPosition = offset;
        }

        CheckFail();
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
        StopVibration();
        Time.timeScale = 1f;

        Debug.Log("<color=green>[STEADY HAND SUCCESS]</color> 5 saniye içeride tuttun!");

        OnSuccess?.Invoke();
    }

    private void Fail()
    {
        isPlaying = false;

        Debug.Log("<color=red>[STEADY HAND FAIL]</color> Dot çemberden çıktı!");

        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
            gamepad.ResetHaptics();
        }

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void StopVibration()
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0f, 0f);
            gamepad.ResetHaptics();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        inputActions?.Dispose();
    }
}