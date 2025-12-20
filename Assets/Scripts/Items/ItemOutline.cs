using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Item outline efekti - Scale ile büyütme
/// </summary>
public class ItemOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float animationSpeed = 5f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHighlighted = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // Smooth scale geçiþi
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void SetOutline(bool active)
    {
        isHighlighted = active;
        targetScale = active ? originalScale * scaleMultiplier : originalScale;
    }
}