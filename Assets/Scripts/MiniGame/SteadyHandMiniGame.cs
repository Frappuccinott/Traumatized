using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SteadyHandMiniGame : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform dot;
    [SerializeField] private RectTransform limitCircle;

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 5f;
    [SerializeField] private float moveSpeed = 300f;

    [Header("Vibration & Noise")]
    [Range(0f, 1f)]
    [SerializeField] private float vibrationStrength = 0.5f;
    [SerializeField] private float noiseStrength = 80f;

    public event Action OnSuccess;
    public event Action OnFail;

    private Vector2 offset;
    private float timer;
    private bool isPlaying;
    private Gamepad gamepad;

    // 🔹 DIŞARIDAN ÇAĞRILACAK
    public void StartGame()
    {
        offset = Vector2.zero;
        dot.anchoredPosition = Vector2.zero;

        timer = 0f;
        isPlaying = true;

        gamepad = Gamepad.current;
        if (gamepad != null)
            gamepad.SetMotorSpeeds(vibrationStrength, vibrationStrength);
    }

    public void StopGame()
    {
        isPlaying = false;
        StopVibration();
    }

    private void Update()
    {
        if (!isPlaying) return;

        timer += Time.deltaTime;
        if (timer >= gameDuration)
        {
            Success();
            return;
        }

        Vector2 input = gamepad != null ? gamepad.leftStick.ReadValue() : Vector2.zero;
        Vector2 noise = UnityEngine.Random.insideUnitCircle * noiseStrength * Time.deltaTime;

        offset += noise + input * moveSpeed * Time.deltaTime;
        dot.anchoredPosition = offset;

        CheckFail();
    }

    private void CheckFail()
    {
        float circleRadius = limitCircle.rect.width * 0.5f;
        float dotRadius = dot.rect.width * 0.5f;

        if (offset.magnitude + dotRadius >= circleRadius)
            Fail();
    }

    private void Success()
    {
        isPlaying = false;
        StopVibration();
        OnSuccess?.Invoke();
    }

    private void Fail()
    {
        isPlaying = false;
        StopVibration();
        OnFail?.Invoke();
    }

    private void StopVibration()
    {
        if (gamepad != null)
            gamepad.SetMotorSpeeds(0f, 0f);
    }
}
