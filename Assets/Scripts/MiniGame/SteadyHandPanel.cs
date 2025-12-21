using UnityEngine;

/// <summary>
/// Steady Hand mini game panel yönetimi
/// Success olunca otomatik kapanır
/// </summary>
public class SteadyHandPanel : MonoBehaviour
{
    [SerializeField] private SteadyHandMiniGame miniGame;

    /// <summary>
    /// Panel'i aç ve mini game'i başlat
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);

        if (miniGame != null)
        {
            miniGame.OnSuccess += OnMiniGameSuccess;
            miniGame.StartGame();
        }
    }

    /// <summary>
    /// Panel'i kapat
    /// </summary>
    public void Hide()
    {
        UnsubscribeEvent();
        gameObject.SetActive(false);
    }

    private void OnMiniGameSuccess()
    {
        Hide();
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