using UnityEngine;

/// <summary>
/// Etkileþim noktasý - Item kullanýlacak yerler
/// Örn: Adam (tabanca hedefi), Su kabý (zehir hedefi), vb.
/// </summary>
[RequireComponent(typeof(ItemOutline))]
public class InteractionPoint : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string pointID = "Target_01";
    [SerializeField] private ItemType requiredItem = ItemType.Gun;
    [SerializeField] private bool requiresItem = true; // Level 5 için false

    private ItemOutline outlineEffect;
    private bool canInteract = true;

    public string PointID => pointID;

    private void Awake()
    {
        outlineEffect = GetComponent<ItemOutline>();
    }

    public bool CanInteract() => canInteract;

    public void SetHighlight(bool active)
    {
        outlineEffect?.SetOutline(active);
    }

    /// <summary>
    /// Etkileþim noktasýna týklandýðýnda
    /// </summary>
    public void Interact(ItemType playerItem)
    {
        if (!canInteract) return;

        // Item gerekiyor mu?
        if (requiresItem)
        {
            // Doðru item'a sahip mi?
            if (playerItem != requiredItem)
            {
                Debug.LogWarning($"Wrong item! Need: {requiredItem}, Have: {playerItem}");
                return;
            }
        }

        canInteract = false;

        // Mini game baþlat
        StartMiniGame();
    }

    private void StartMiniGame()
    {
        if (MiniGameBarManager.Instance != null)
        {
            MiniGameBarManager.Instance.StartMiniGame(requiredItem, OnMiniGameSuccess, OnMiniGameFail);
        }
    }

    private void OnMiniGameSuccess()
    {
        Debug.Log($"SUCCESS: {requiredItem} used at {pointID}");

        // TODO: Animasyon/Cutscene
        // Item'ý kullan
        PlayerInteraction player = FindFirstObjectByType<PlayerInteraction>();
        player?.UseItem();

        // Level geçiþi veya devam
        // LevelManager.Instance?.OnQTESuccess();
    }

    private void OnMiniGameFail()
    {
        Debug.Log($"FAIL: {requiredItem} at {pointID}");

        // TODO: Fail durumu
        // LevelManager.Instance?.OnQTEFail();
    }
}