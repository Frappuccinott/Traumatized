using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Item Settings")]
    [SerializeField] private string itemID = "Item_01";
    [SerializeField] private ItemOutline outlineEffect;

    private bool canInteract = true;

    private void Awake()
    {
        if (outlineEffect == null)
        {
            outlineEffect = GetComponent<ItemOutline>();
        }
    }

    public string ItemID => itemID;

    public bool CanInteract()
    {
        return canInteract;
    }

    public void SetHighlight(bool active)
    {
        if (outlineEffect != null)
        {
            outlineEffect.SetOutline(active);
        }
    }

    public void OnCollected()
    {
        canInteract = false;
        Destroy(gameObject);
    }
}