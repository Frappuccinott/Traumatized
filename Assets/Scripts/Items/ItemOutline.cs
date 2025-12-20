using UnityEngine;

public class ItemOutline : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = Color.yellow;
    [SerializeField] private float outlineWidth = 0.1f;
    [SerializeField] private bool animateOutline = true;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minWidth = 0.05f;
    [SerializeField] private float maxWidth = 0.15f;

    private Renderer[] renderers;
    private MaterialPropertyBlock propertyBlock;
    private bool isOutlineActive = false;
    private float animationTime = 0f;

    private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
    private static readonly int OutlineWidthProperty = Shader.PropertyToID("_OutlineWidth");

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        // Baþlangýçta outline kapalý
        SetOutline(false);
    }

    private void Update()
    {
        if (isOutlineActive && animateOutline)
        {
            animationTime += Time.deltaTime * pulseSpeed;
            float width = Mathf.Lerp(minWidth, maxWidth, (Mathf.Sin(animationTime) + 1f) / 2f);
            UpdateOutlineWidth(width);
        }
    }

    public void SetOutline(bool active)
    {
        isOutlineActive = active;

        if (active)
        {
            animationTime = 0f;
            UpdateOutline(outlineColor, outlineWidth);
        }
        else
        {
            UpdateOutline(Color.clear, 0f);
        }
    }

    private void UpdateOutline(Color color, float width)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(OutlineColorProperty, color);
                propertyBlock.SetFloat(OutlineWidthProperty, width);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }

    private void UpdateOutlineWidth(float width)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(OutlineWidthProperty, width);
                renderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}