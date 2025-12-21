using UnityEngine;

/// <summary>
/// Level geçiþ trigger'ý - Oyuncu collider'a girince sonraki level'a geç
/// </summary>
public class LevelTransitionTrigger : MonoBehaviour
{
    [Header("Next Level")]
    [SerializeField] private string nextLevelName = "Level2";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        }
    }
}