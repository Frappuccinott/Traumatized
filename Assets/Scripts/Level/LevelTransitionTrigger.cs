using UnityEngine;

/// <summary>
/// Level geçiþ trigger'ý - Steady Hand baþarýlý olmadan geçemez
/// </summary>
public class LevelTransitionTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            TryLoadNextLevel();
        }
    }

    private void TryLoadNextLevel()
    {
        if (LevelManager.Instance == null) return;

        // Steady Hand baþarýlý olmadan geçemez
        if (!LevelManager.Instance.CanProgressToNextLevel())
        {
            return;
        }

        hasTriggered = true;
        LevelManager.Instance.LoadNextLevel();
    }
}