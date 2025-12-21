using UnityEngine;

/// <summary>
/// Toplanabilir item - Elle tutulan itemler
/// PlayerInteraction tarafýndan pickup edilir
/// </summary>
[RequireComponent(typeof(ItemOutline))]
public class InteractableItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private string itemID = "Item_01";
    [SerializeField] private ItemType itemType = ItemType.Gun;
    [SerializeField] private GameObject itemHandModel;

    private ItemOutline outlineEffect;
    private bool canInteract = true;

    public string ItemID => itemID;
    public ItemType ItemType => itemType;

    private void Awake()
    {
        outlineEffect = GetComponent<ItemOutline>();
    }

    public bool CanInteract() => canInteract;

    /// <summary>
    /// PlayerInteraction tarafýndan hover olunca çaðrýlýr
    /// </summary>
    public void SetHighlight(bool active)
    {
        outlineEffect?.SetOutline(active);
    }

    /// <summary>
    /// PlayerInteraction tarafýndan týklanýnca çaðrýlýr
    /// </summary>
    public void Interact()
    {
        if (!canInteract) return;

        canInteract = false;
        CollectItem();
    }

    private void CollectItem()
    {
        PlayerInteraction player = FindFirstObjectByType<PlayerInteraction>();

        if (player != null)
        {
            player.PickupItem(itemType, itemHandModel);
        }

        Destroy(gameObject);
    }
}