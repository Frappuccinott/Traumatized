using UnityEngine;

public class SteadyHandPanel : MonoBehaviour
{
    [SerializeField] private SteadyHandMiniGame miniGame;

    private void Awake()
    {
        gameObject.SetActive(false);

        miniGame.OnSuccess += HandleSuccess;
        miniGame.OnFail += HandleFail;
    }

    // 🔹 ŞİMDİ TEST İÇİN BURADAN AÇACAĞIZ
    public void Show()
    {
        gameObject.SetActive(true);
        miniGame.StartGame();
    }

    public void Hide()
    {
        miniGame.StopGame();
        gameObject.SetActive(false);
    }

    private void HandleSuccess()
    {
        Debug.Log("STEADY HAND: SUCCESS");
        Hide();
    }

    private void HandleFail()
    {
        Debug.Log("STEADY HAND: FAIL");
        Hide();
    }
}
