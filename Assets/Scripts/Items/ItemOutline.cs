using UnityEngine;

/// <summary>
/// Item outline efekti - Hover olunca scale ile büyütme animasyonu
/// </summary>
public class ItemOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    /// <summary>
    /// InteractableItem ve InteractionPoint tarafýndan çaðrýlýr
    /// </summary>
    public void SetOutline(bool active)
    {
        targetScale = active ? originalScale * scaleMultiplier : originalScale;
    }
}