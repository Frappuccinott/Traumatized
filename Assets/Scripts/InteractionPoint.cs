using UnityEngine;

/// <summary>
/// Etkileþim noktasý - Her level'da farklý kill animation
/// </summary>
[RequireComponent(typeof(ItemOutline))]
public class InteractionPoint : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string pointID = "Target_01";
    [SerializeField] private ItemType requiredItem = ItemType.Gun;
    [SerializeField] private bool requiresItem = true;

    [Header("Kill Animation")]
    [SerializeField] private CutscenePlayer killAnimation; // Level'a özel
    [SerializeField] private EnemyController enemyController; // Bu enemy

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

    public void Interact(ItemType playerItem)
    {
        if (!canInteract) return;
        if (!ValidateItem(playerItem)) return;

        canInteract = false;
        StartMiniGame();
    }

    private bool ValidateItem(ItemType playerItem)
    {
        if (!requiresItem) return true;
        return playerItem == requiredItem;
    }

    private void StartMiniGame()
    {
        MiniGameBarManager.Instance?.StartMiniGame(
            requiredItem,
            OnMiniGameSuccess,
            OnMiniGameFail,
            killAnimation,
            enemyController
        );
    }

    private void OnMiniGameSuccess()
    {
        UsePlayerItem();
    }

    private void OnMiniGameFail()
    {
        // MiniGameBarManager zaten handle ediyor
    }

    private void UsePlayerItem()
    {
        FindFirstObjectByType<PlayerInteraction>()?.UseItem();
    }
}