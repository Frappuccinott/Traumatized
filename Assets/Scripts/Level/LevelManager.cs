using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Level yönetimi - Sahne geçiþleri
/// Steady Hand baþarýlý olmadan sonraki level'a geçemez
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private string nextLevelName;

    private bool canProgressToNextLevel = false;

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
    /// Steady Hand baþarýlý - Level geçiþine izin ver
    /// </summary>
    public void UnlockLevelProgression()
    {
        canProgressToNextLevel = true;
    }

    /// <summary>
    /// Level geçiþi yapýlabilir mi?
    /// </summary>
    public bool CanProgressToNextLevel() => canProgressToNextLevel;

    /// <summary>
    /// Mevcut sahneyi yeniden yükle
    /// </summary>
    public void RestartLevel()
    {
        ClearPlayerInventory();
        ReloadCurrentScene();
    }

    /// <summary>
    /// Sonraki level'a geç (sadece unlock olduysa)
    /// </summary>
    public void LoadNextLevel()
    {
        if (!canProgressToNextLevel)
        {
            return;
        }

        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
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