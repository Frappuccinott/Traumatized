using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Panel Referansları")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private bool isPaused;
    private bool pauseWasActiveBeforeSettings;

    private PlayerInputActions inputActions;
    private InputAction pauseAction;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        pauseAction = inputActions.UI.Pause;

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePressed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
    }

    private void Start()
    {
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);

        // Cursor her zaman görünür olacak
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetPauseState(false); // Game zamanını normal bırak
    }


    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    private void SetPauseState(bool pause)
    {
        Time.timeScale = pause ? 0f : 1f;
        isPaused = pause;

        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void PauseGame()
    {
        if (pausePanel == null) return;

        pausePanel.SetActive(true);
        settingsPanel?.SetActive(false);
        SetPauseState(true);
    }

    public void ResumeGame()
    {
        pausePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        SetPauseState(false);
    }

    public void ClosePausePanel()
    {
        pausePanel.SetActive(false);
    }

    public void OpenSettings()
    {
        if (settingsPanel == null || pausePanel == null) return;

        pauseWasActiveBeforeSettings = pausePanel.activeSelf;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        settingsPanel.transform.SetAsLastSibling();
    }

    public void CloseSettings()
    {
        if (settingsPanel == null || pausePanel == null) return;

        settingsPanel.SetActive(false);

        if (pauseWasActiveBeforeSettings)
        {
            pausePanel.SetActive(true);
            SetPauseState(true);
        }
        else
        {
            SetPauseState(false);
        }
    }

    public void BackMainMenu()
    {
        Time.timeScale = 1f;
        inputActions?.Disable();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        inputActions?.Disable();
    }
}