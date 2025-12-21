using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Fail olunca kırmızı ekran efekti - UnscaledTime kullanır
/// </summary>
public class RedScreenEffect : MonoBehaviour
{
    public static RedScreenEffect Instance { get; private set; }

    [Header("Red Screen Settings")]
    [SerializeField] private Image redOverlay;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private Color redColor = new Color(1f, 0f, 0f, 0.5f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (redOverlay != null)
        {
            Color transparent = new Color(redColor.r, redColor.g, redColor.b, 0f);
            redOverlay.color = transparent;
            redOverlay.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Kırmızı ekran göster
    /// </summary>
    public void ShowRedScreen()
    {
        if (redOverlay != null)
        {
            StopAllCoroutines();
            StartCoroutine(FadeInRed());
        }
    }

    private IEnumerator FadeInRed()
    {
        redOverlay.gameObject.SetActive(true);

        float elapsed = 0f;
        Color startColor = new Color(redColor.r, redColor.g, redColor.b, 0f);

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime; // ⬅️ UnscaledTime (Time.timeScale = 0 olsa bile çalışır)
            float t = elapsed / fadeInDuration;
            float alpha = Mathf.Lerp(0f, redColor.a, t);
            redOverlay.color = new Color(redColor.r, redColor.g, redColor.b, alpha);
            yield return null;
        }

        redOverlay.color = redColor;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}