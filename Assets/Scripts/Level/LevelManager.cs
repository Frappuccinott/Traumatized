using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level yönetimi - Sahne geçiþleri ve cutscene oynatma
/// Singleton pattern ile global eriþim
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private string nextLevelName;

    [Header("Cutscene Settings")]
    [SerializeField] private CutscenePlayer successCutscene;
    [SerializeField] private CutscenePlayer failCutscene;

    private bool isTransitioning;

    private void Awake()
    {
        InitializeSingleton();
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

    /// <summary>
    /// Mini game baþarýlý - Success cutscene oynat
    /// </summary>
    public void OnQTESuccess()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        if (successCutscene != null)
        {
            successCutscene.PlayCutscene(OnSuccessCutsceneEnd);
        }
        else
        {
            OnSuccessCutsceneEnd();
        }
    }

    /// <summary>
    /// Mini game baþarýsýz - Fail cutscene oynat ve restart
    /// </summary>
    public void OnQTEFail()
    {
        if (isTransitioning) return;

        isTransitioning = true;

        if (failCutscene != null)
        {
            failCutscene.PlayCutscene(RestartLevel);
        }
        else
        {
            RestartLevel();
        }
    }

    private void OnSuccessCutsceneEnd()
    {
        EnablePlayerMovement();
        isTransitioning = false;
    }

    /// <summary>
    /// Mevcut sahneyi yeniden yükle
    /// </summary>
    public void RestartLevel()
    {
        ClearPlayerInventory();
        ReloadCurrentScene();
    }

    /// <summary>
    /// Sonraki level'a geç
    /// </summary>
    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    private void EnablePlayerMovement()
    {
        FindFirstObjectByType<PlayerController>()?.EnableMovement();
    }

    private void ClearPlayerInventory()
    {
        FindFirstObjectByType<PlayerInteraction>()?.ClearInventory();
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}