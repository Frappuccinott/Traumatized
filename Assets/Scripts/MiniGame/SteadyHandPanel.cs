using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Steady Hand panel - Success → Player hareket edebilir, panel kapanır
/// </summary>
public class SteadyHandPanel : MonoBehaviour
{
    [SerializeField] private SteadyHandMiniGame miniGame;

    public void Show()
    {
        gameObject.SetActive(true);

        if (miniGame != null)
        {
            miniGame.OnSuccess += OnMiniGameSuccess;
            miniGame.StartGame();
        }
    }

    public void Hide()
    {
        UnsubscribeEvent();
        gameObject.SetActive(false);
    }

    private void OnMiniGameSuccess()
    {
        // Player hareket edebilir
        EnablePlayerMovement();

        // Panel kapat
        Hide();
    }

    private void EnablePlayerMovement()
    {
        FindFirstObjectByType<PlayerController>()?.EnableMovement();
    }

    private void UnsubscribeEvent()
    {
        if (miniGame != null)
        {
            miniGame.OnSuccess -= OnMiniGameSuccess;
        }
    }

    private void OnDisable()
    {
        UnsubscribeEvent();
    }

    private void OnDestroy()
    {
        UnsubscribeEvent();
    }
}