//using UnityEngine;

///// <summary>
///// Etkileþilebilir item - Cursor ile highlight ve toplama
///// Týklanýnca direkt toplanýr ve inventory'e eklenir
///// </summary>
//[RequireComponent(typeof(ItemOutline))]
//public class InteractableItem : MonoBehaviour
//{
//    [Header("Item Settings")]
//    [SerializeField] private string itemID = "Item_01";

//    private ItemOutline outlineEffect;
//    private bool canInteract = true;

//    public string ItemID => itemID;

//    private void Awake()
//    {
//        outlineEffect = GetComponent<ItemOutline>();
//    }

//    public bool CanInteract() => canInteract;

//    /// <summary>
//    /// Cursor hover olduðunda highlight aç/kapat
//    /// </summary>
//    public void SetHighlight(bool active)
//    {
//        outlineEffect?.SetOutline(active);
//    }

//    /// <summary>
//    /// Item'a týklandýðýnda çaðrýlýr
//    /// </summary>
//    public void Interact()
//    {
//        if (!canInteract) return;

//        Collect();
//    }

//    private void Collect()
//    {
//        canInteract = false;

//        // PlayerInteraction'a item ekle
//        PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
//        playerInteraction?.AddItem(itemID);

//        Destroy(gameObject);
//    }
//}

using UnityEngine;

/// <summary>
/// Toplanabilir item - Elle tutulan itemler
/// </summary>
[RequireComponent(typeof(ItemOutline))]
public class InteractableItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private string itemID = "Item_01";
    [SerializeField] private ItemType itemType = ItemType.Gun;
    [SerializeField] private GameObject itemHandModel; // Elle tutulan model

    private ItemOutline outlineEffect;
    private bool canInteract = true;

    public string ItemID => itemID;
    public ItemType ItemType => itemType;

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
    /// Item'a týklandýðýnda
    /// </summary>
    public void Interact()
    {
        if (!canInteract) return;

        canInteract = false;

        // Item'ý topla
        CollectItem();
    }

    private void CollectItem()
    {
        // PlayerInteraction'a item ekle
        PlayerInteraction playerInteraction = FindFirstObjectByType<PlayerInteraction>();
        if (playerInteraction != null)
        {
            playerInteraction.PickupItem(itemType, itemHandModel);
        }

        // Sahnedeki item'ý yok et
        Destroy(gameObject);
    }
}