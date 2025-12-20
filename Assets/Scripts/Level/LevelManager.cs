using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Settings")]
    [SerializeField] private string currentLevelName;
    [SerializeField] private string nextLevelName;

    [Header("Cutscene Settings")]
    [SerializeField] private CutscenePlayer successCutscene;
    [SerializeField] private CutscenePlayer failCutscene;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void OnQTESuccess()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Success cutscene oynat
        if (successCutscene != null)
        {
            successCutscene.PlayCutscene(() => OnSuccessCutsceneEnd());
        }
        else
        {
            OnSuccessCutsceneEnd();
        }
    }

    public void OnQTEFail()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        // Fail cutscene oynat
        if (failCutscene != null)
        {
            failCutscene.PlayCutscene(() => OnFailCutsceneEnd());
        }
        else
        {
            OnFailCutsceneEnd();
        }
    }

    private void OnSuccessCutsceneEnd()
    {
        // Oyuncu kontrolü geri ver
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.EnableMovement();
        }

        isTransitioning = false;

        // Level geçiþi manuel olacak (LevelTransition trigger ile)
    }

    private void OnFailCutsceneEnd()
    {
        // Sahneyi yeniden baþlat
        RestartLevel();
    }

    public void RestartLevel()
    {
        // Player inventory temizle
        PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.ClearItems();
        }

        // Sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            Debug.LogWarning("Next level name is not set!");
        }
    }
}