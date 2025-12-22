using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Bar mini game - Success → Kill animation → Enemy die → Steady hand
/// Fail → Red screen → Reload
/// </summary>
public class MiniGameBarManager : MonoBehaviour
{
    public static MiniGameBarManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject miniGamePanel;
    [SerializeField] private RectTransform slider;
    [SerializeField] private RectTransform barBackground;
    [SerializeField] private RectTransform greenZone;

    [Header("Game Settings")]
    [SerializeField] private float sliderSpeed = 300f;

    [Header("Audio")]
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;

    [Header("Next MiniGame")]
    [SerializeField] private SteadyHandPanel steadyHandPanel;

    private InputAction confirmAction;
    private AudioSource audioSource;
    private float barWidth;
    private float sliderPosition;
    private bool isGameActive;
    private bool hasPressedButton;
    private ItemType currentItemType;
    private CutscenePlayer currentKillAnimation;
    private EnemyController currentEnemy;

    private void Awake()
    {
        InitializeSingleton();
        InitializeAudio();
        InitializeInput();
    }

    private void InitializeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    private void InitializeInput()
    {
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

        if (barBackground != null)
        {
            barWidth = barBackground.rect.width;
        }
    }

    private void Update()
    {
        if (!isGameActive) return;

        UpdateSlider();
        CheckInput();
    }

    public void StartMiniGame(
        ItemType itemType,
        Action onSuccess,
        Action onFail,
        CutscenePlayer killAnimation,
        EnemyController enemy)
    {
        currentItemType = itemType;
        currentKillAnimation = killAnimation;
        currentEnemy = enemy;

        miniGamePanel?.SetActive(true);
        DisablePlayerMovement();
        ResetGameState();

        confirmAction.Disable();
        confirmAction.Enable();
    }

    private void ResetGameState()
    {
        sliderPosition = 0f;
        hasPressedButton = false;
        isGameActive = true;

        if (slider != null)
        {
            slider.anchoredPosition = new Vector2(0f, slider.anchoredPosition.y);
        }
    }

    private void UpdateSlider()
    {
        sliderPosition += sliderSpeed * Time.deltaTime;
        float xPos = Mathf.Min(sliderPosition, barWidth);

        if (slider != null)
        {
            slider.anchoredPosition = new Vector2(xPos, slider.anchoredPosition.y);
        }

        if (sliderPosition >= barWidth && !hasPressedButton)
        {
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
            {
                OnSuccess();
            }
            else
            {
                OnFail();
            }
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

    private void OnSuccess()
    {
        PlaySound(successSound);
        EndMiniGame();
        PlayKillAnimation();
    }

    private void OnFail()
    {
        PlaySound(failSound);
        EndMiniGame();
        ShowRedScreenAndReload();
    }

    private void EndMiniGame()
    {
        isGameActive = false;
        miniGamePanel?.SetActive(false);
    }

    private void PlayKillAnimation()
    {
        if (currentKillAnimation != null)
        {
            currentKillAnimation.PlayCutscene(OnKillAnimationEnd);
        }
        else
        {
            OnKillAnimationEnd();
        }
    }

    private void OnKillAnimationEnd()
    {
        currentEnemy?.SetDeathTransform();
        steadyHandPanel?.Show();
    }

    private void ShowRedScreenAndReload()
    {
        RedScreenEffect.Instance?.ShowRedScreen();
        Invoke(nameof(ReloadScene), 1f);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void DisablePlayerMovement()
    {
        FindFirstObjectByType<PlayerController>()?.DisableMovement();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}