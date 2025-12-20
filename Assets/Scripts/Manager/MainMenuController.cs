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

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Play Video")]
    [SerializeField] private VideoPlayer playVideo;

    private bool isPlaying = false;

    private void Start()
    {
        if (playVideo != null)
            playVideo.Prepare();
    }

    // ================= PLAY GAME =================
    public void PlayGame()
    {
        if (isPlaying) return;
        isPlaying = true;

        menuCanvas.SetActive(false);
        videoPanel.SetActive(true);

        playVideo.loopPointReached += OnPlayVideoFinished;

        if (playVideo.isPrepared)
            playVideo.Play();
        else
        {
            playVideo.prepareCompleted += OnPlayVideoPrepared;
            playVideo.Prepare();
        }
    }

    private void OnPlayVideoPrepared(VideoPlayer vp)
    {
        playVideo.prepareCompleted -= OnPlayVideoPrepared;
        playVideo.Play();
    }

    private void OnPlayVideoFinished(VideoPlayer vp)
    {
        playVideo.loopPointReached -= OnPlayVideoFinished;
        SceneManager.LoadScene(gameSceneName);
    }

    // ================= QUIT GAME =================
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ================= SETTINGS =================
    public void OpenSetPanel() => settingsPanel?.SetActive(true);
    public void CloseSetPanel() => settingsPanel?.SetActive(false);
}
