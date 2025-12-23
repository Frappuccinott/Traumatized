using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Final sahne yöneticisi - Otomatik cutscene oynat, sonra ana menüye dön
/// </summary>
public class FinalCutsceneManager : MonoBehaviour
{
    [Header("Final Cutscene")]
    [SerializeField] private CutscenePlayer finalCutscene;

    [Header("Return Settings")]
    [SerializeField] private int mainMenuSceneIndex = 0;

    private void Start()
    {
        DisablePlayerControls();
        PlayFinalCutscene();
    }

    private void PlayFinalCutscene()
    {
        if (finalCutscene != null)
        {
            finalCutscene.PlayCutscene(OnCutsceneEnd);
        }
        else
        {
            OnCutsceneEnd();
        }
    }

    private void OnCutsceneEnd()
    {
        ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }

    private void DisablePlayerControls()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.DisableMovement();
        }
    }
}