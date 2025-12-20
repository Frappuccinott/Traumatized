using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip correctKeySound;
    [SerializeField] private AudioClip wrongKeySound;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    private AudioSource audioSource;
    private QTESequence currentSequence;
    private List<GameObject> buttonIcons = new List<GameObject>();
    private int currentIndex = 0;
    private float timeRemaining;
    private bool isQTEActive = false;
    private bool isUsingGamepad = false;

    private Action onSuccessCallback;
    private Action onFailCallback;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        inputActions = new PlayerInputActions();

        if (qtePanel != null)
        {
            qtePanel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        if (!isQTEActive) return;

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        if (timeRemaining <= 0)
        {
            FailQTE();
            return;
        }

        CheckInput();
    }

    public void StartQTE(QTESequence sequence, Action onSuccess, Action onFail)
    {
        currentSequence = sequence;
        onSuccessCallback = onSuccess;
        onFailCallback = onFail;
        currentIndex = 0;
        timeRemaining = sequence.TimeLimit;
        isQTEActive = true;

        isUsingGamepad = Gamepad.current != null;

        SetupUI();

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.DisableMovement();
        }

        if (qtePanel != null)
        {
            qtePanel.SetActive(true);
        }
    }

    private void SetupUI()
    {
        foreach (GameObject icon in buttonIcons)
        {
            Destroy(icon);
        }
        buttonIcons.Clear();

        if (isUsingGamepad)
        {
            foreach (GamepadButton button in currentSequence.GamepadButtons)
            {
                GameObject icon = Instantiate(buttonPrefab, buttonContainer);
                TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = GetGamepadButtonSymbol(button);
                }
                buttonIcons.Add(icon);
            }
        }
        else
        {
            foreach (KeyCode key in currentSequence.KeyboardKeys)
            {
                GameObject icon = Instantiate(buttonPrefab, buttonContainer);
                TextMeshProUGUI text = icon.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = key.ToString();
                }
                buttonIcons.Add(icon);
            }
        }

        UpdateButtonVisuals();
    }

    private void CheckInput()
    {
        if (isUsingGamepad)
        {
            CheckGamepadInput();
        }
        else
        {
            CheckKeyboardInput();
        }
    }

    private void CheckKeyboardInput()
    {
        if (currentIndex >= currentSequence.KeyboardKeys.Count) return;

        KeyCode expectedKey = currentSequence.KeyboardKeys[currentIndex];

        if (Keyboard.current[GetKey(expectedKey)].wasPressedThisFrame)
        {
            CorrectInput();
        }
        else if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            WrongInput();
        }
    }

    private void CheckGamepadInput()
    {
        if (currentIndex >= currentSequence.GamepadButtons.Count) return;
        if (Gamepad.current == null) return;

        GamepadButton expectedButton = currentSequence.GamepadButtons[currentIndex];

        if (IsGamepadButtonPressed(expectedButton))
        {
            CorrectInput();
        }
    }

    private bool IsGamepadButtonPressed(GamepadButton button)
    {
        Gamepad gamepad = Gamepad.current;

        return button switch
        {
            GamepadButton.South => gamepad.buttonSouth.wasPressedThisFrame,
            GamepadButton.East => gamepad.buttonEast.wasPressedThisFrame,
            GamepadButton.West => gamepad.buttonWest.wasPressedThisFrame,
            GamepadButton.North => gamepad.buttonNorth.wasPressedThisFrame,
            GamepadButton.LeftShoulder => gamepad.leftShoulder.wasPressedThisFrame,
            GamepadButton.RightShoulder => gamepad.rightShoulder.wasPressedThisFrame,
            GamepadButton.LeftTrigger => gamepad.leftTrigger.wasPressedThisFrame,
            GamepadButton.RightTrigger => gamepad.rightTrigger.wasPressedThisFrame,
            GamepadButton.Start => gamepad.startButton.wasPressedThisFrame,
            GamepadButton.Select => gamepad.selectButton.wasPressedThisFrame,
            _ => false
        };
    }

    private void CorrectInput()
    {
        if (correctKeySound != null)
        {
            audioSource.PlayOneShot(correctKeySound);
        }

        currentIndex++;
        UpdateButtonVisuals();

        int totalKeys = isUsingGamepad ? currentSequence.GamepadButtons.Count : currentSequence.KeyboardKeys.Count;
        if (currentIndex >= totalKeys)
        {
            SuccessQTE();
        }
    }

    private void WrongInput()
    {
        if (wrongKeySound != null)
        {
            audioSource.PlayOneShot(wrongKeySound);
        }

        FailQTE();
    }

    private void SuccessQTE()
    {
        if (successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        EndQTE();
        onSuccessCallback?.Invoke();
    }

    private void FailQTE()
    {
        if (failSound != null)
        {
            audioSource.PlayOneShot(failSound);
        }

        EndQTE();
        onFailCallback?.Invoke();
    }

    private void EndQTE()
    {
        isQTEActive = false;

        if (qtePanel != null)
        {
            qtePanel.SetActive(false);
        }

        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"{timeRemaining:F1}s";
        }
    }

    private void UpdateButtonVisuals()
    {
        for (int i = 0; i < buttonIcons.Count; i++)
        {
            Image image = buttonIcons[i].GetComponent<Image>();
            if (image != null)
            {
                image.color = i < currentIndex ? Color.green : (i == currentIndex ? Color.yellow : Color.white);
            }
        }
    }

    private string GetGamepadButtonSymbol(GamepadButton button)
    {
        return button switch
        {
            GamepadButton.South => "A",
            GamepadButton.East => "B",
            GamepadButton.West => "X",
            GamepadButton.North => "Y",
            GamepadButton.LeftShoulder => "LB",
            GamepadButton.RightShoulder => "RB",
            GamepadButton.LeftTrigger => "LT",
            GamepadButton.RightTrigger => "RT",
            GamepadButton.Start => "Start",
            GamepadButton.Select => "Select",
            _ => "?"
        };
    }

    private Key GetKey(KeyCode keyCode)
    {
        return keyCode switch
        {
            KeyCode.A => Key.A,
            KeyCode.B => Key.B,
            KeyCode.C => Key.C,
            KeyCode.D => Key.D,
            KeyCode.E => Key.E,
            KeyCode.F => Key.F,
            KeyCode.G => Key.G,
            KeyCode.H => Key.H,
            KeyCode.I => Key.I,
            KeyCode.J => Key.J,
            KeyCode.K => Key.K,
            KeyCode.L => Key.L,
            KeyCode.M => Key.M,
            KeyCode.N => Key.N,
            KeyCode.O => Key.O,
            KeyCode.P => Key.P,
            KeyCode.Q => Key.Q,
            KeyCode.R => Key.R,
            KeyCode.S => Key.S,
            KeyCode.T => Key.T,
            KeyCode.U => Key.U,
            KeyCode.V => Key.V,
            KeyCode.W => Key.W,
            KeyCode.X => Key.X,
            KeyCode.Y => Key.Y,
            KeyCode.Z => Key.Z,
            KeyCode.Space => Key.Space,
            _ => Key.Space
        };
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
        if (Instance == this) Instance = null;
    }
}