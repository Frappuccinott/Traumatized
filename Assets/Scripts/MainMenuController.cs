using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject videoPanel;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Quit Video")]
    [SerializeField] private VideoPlayer quitVideo;

    private bool isQuitting = false;

    private void Start()
    {
        if (quitVideo != null)
        {
            quitVideo.Prepare();
        }
    }

    public void PlayGame()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game scene name is not set!");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        if (isQuitting) return;

        if (quitVideo == null)
        {
            QuitApplication();
            return;
        }

        isQuitting = true;

        menuCanvas.SetActive(false);
        videoPanel.SetActive(true);

        quitVideo.loopPointReached += OnQuitVideoFinished;

        if (quitVideo.isPrepared)
        {
            quitVideo.Play();
        }
        else
        {
            quitVideo.prepareCompleted += OnVideoPrepared;
            quitVideo.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        quitVideo.prepareCompleted -= OnVideoPrepared;
        quitVideo.Play();
    }

    private void OnQuitVideoFinished(VideoPlayer vp)
    {
        quitVideo.loopPointReached -= OnQuitVideoFinished;
        QuitApplication();
    }

    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        if (quitVideo != null)
        {
            quitVideo.loopPointReached -= OnQuitVideoFinished;
            quitVideo.prepareCompleted -= OnVideoPrepared;
        }
    }

    public void OpenSetPanel() => settingsPanel?.SetActive(true);
    public void CloseSetPanel() => settingsPanel?.SetActive(false);
    public void OpenCreditsPanel() => creditsPanel?.SetActive(true);
    public void CloseCreditsPanel() => creditsPanel?.SetActive(false);
}