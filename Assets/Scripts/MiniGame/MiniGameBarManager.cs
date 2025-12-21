using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class MiniGameBarManager : MonoBehaviour
{
    public static MiniGameBarManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject miniGamePanel;
    [SerializeField] private RectTransform slider;
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private RectTransform greenZone;
    [SerializeField] private TMP_Text instructionText;

    [Header("Game Settings")]
    [SerializeField] private float sliderSpeed = 300f;

    [Header("Audio")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    [Header("Transition Video")]
    [SerializeField] private GameObject videoPanel;
    [SerializeField] private VideoPlayer transitionVideo;

    [Header("Next MiniGame")]
    [SerializeField] private SteadyHandPanel steadyHandPanel;

    private InputAction confirmAction;
    private AudioSource audioSource;

    private float barWidth;
    private float sliderPosition;
    private bool isGameActive;
    private bool hasPressedButton;

    private Action onSuccessCallback;
    private Action onFailCallback;
    private ItemType currentItemType;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();

        confirmAction = new InputAction("MiniGameConfirm", InputActionType.Button);
        confirmAction.AddBinding("<Keyboard>/e");
        confirmAction.AddBinding("<Gamepad>/buttonWest");
    }

    private void OnEnable()
    {
        confirmAction.Enable();
    }

    private void OnDisable()
    {
        confirmAction.Disable();
    }

    private void Start()
    {
        miniGamePanel?.SetActive(false);
        videoPanel?.SetActive(false);

        if (barBackground != null)
            barWidth = barBackground.rect.width;

        if (transitionVideo != null)
            transitionVideo.loopPointReached += OnTransitionVideoFinished;
    }

    private void Update()
    {
        if (!isGameActive) return;

        MoveSlider();
        CheckInput();
    }

    // ================= START =================
    public void StartMiniGame(ItemType itemType, Action onSuccess, Action onFail)
    {
        currentItemType = itemType;
        onSuccessCallback = onSuccess;
        onFailCallback = onFail;

        miniGamePanel?.SetActive(true);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.DisableMovement();

        sliderPosition = 0f;
        hasPressedButton = false;
        isGameActive = true;

        if (slider != null)
            slider.anchoredPosition = new Vector2(0f, slider.anchoredPosition.y);

        UpdateInstructionText();

        confirmAction.Disable();
        confirmAction.Enable();
    }

    // ================= BAR LOGIC =================
    private void MoveSlider()
    {
        sliderPosition += sliderSpeed * Time.deltaTime;

        float xPos = Mathf.Min(sliderPosition, barWidth);

        if (slider != null)
            slider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);

        if (sliderPosition >= barWidth && !hasPressedButton)
        {
            Debug.Log("<color=red>[MINIGAME FAIL]</color> No input");
            OnFail();
        }
    }

    private void CheckInput()
    {
        if (hasPressedButton) return;

        if (confirmAction.WasPressedThisFrame())
        {
            hasPressedButton = true;

            if (IsInGreenZone())
                OnSuccess();
            else
                OnFail();
        }
    }

    private bool IsInGreenZone()
    {
        if (greenZone == null || barBackground == null) return false;

        float greenCenterX = greenZone.anchoredPosition.x + barWidth / 2f;
        float greenHalfWidth = greenZone.rect.width / 2f;

        return sliderPosition >= greenCenterX - greenHalfWidth &&
               sliderPosition <= greenCenterX + greenHalfWidth;
    }

    // ================= RESULT =================
    private void OnSuccess()
    {
        Debug.Log($"<color=green>[BAR MINIGAME SUCCESS]</color> Item: {currentItemType}");

        if (successSound != null)
            audioSource.PlayOneShot(successSound);

        EndMiniGame();
        PlayTransitionVideo();
    }

    private void OnFail()
    {
        if (failSound != null)
            audioSource.PlayOneShot(failSound);

        EndMiniGame();

        Debug.Log("<color=red>[BAR MINIGAME FAIL]</color> Reloading scene");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void EndMiniGame()
    {
        isGameActive = false;
        miniGamePanel?.SetActive(false);

        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.EnableMovement();
    }

    // ================= VIDEO =================
    private void PlayTransitionVideo()
    {
        if (transitionVideo == null || videoPanel == null)
        {
            Debug.Log("No transition video → SteadyHand starts");
            steadyHandPanel?.Show();
            return;
        }

        videoPanel.SetActive(true);
        transitionVideo.Stop();
        transitionVideo.Play();
    }

    private void OnTransitionVideoFinished(VideoPlayer vp)
    {
        videoPanel.SetActive(false);

        Debug.Log("<color=cyan>[TRANSITION VIDEO FINISHED]</color>");

        steadyHandPanel?.Show();
    }

    private void UpdateInstructionText()
    {
        if (instructionText != null)
            instructionText.text =
                $"Press X when the line is in the green zone ({currentItemType})!";
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        if (transitionVideo != null)
            transitionVideo.loopPointReached -= OnTransitionVideoFinished;
    }
}
