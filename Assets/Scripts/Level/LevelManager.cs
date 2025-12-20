using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level yönetimi - Sahne geçiþleri, cutscene oynatma ve restart
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

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// QTE baþarýlý - Success cutscene oynat ve devam et
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
    /// QTE baþarýsýz - Fail cutscene oynat ve restart
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
        // Oyuncu kontrolü geri ver
        PlayerController player = FindFirstObjectByType<PlayerController>();
        player?.EnableMovement();

        isTransitioning = false;
    }

    /// <summary>
    /// Mevcut sahneyi yeniden yükle
    /// </summary>
    public void RestartLevel()
    {
        // Inventory temizle
        PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        playerInteraction?.ClearInventory();

        // Sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Bir sonraki level'a geç
    /// </summary>
    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("LevelManager: Next level name is not set!");
        }
    }
}   