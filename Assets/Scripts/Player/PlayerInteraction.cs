using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Oyuncu etkileþim sistemi - Virtual cursor ile item toplama ve interaction point kullanma
/// InteractableItem ve InteractionPoint ile çalýþýr
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float raycastRange = 50f;
    [SerializeField] private LayerMask interactableMask;

    [Header("Virtual Cursor")]
    [SerializeField] private VirtualCursor virtualCursor;

    [Header("Hand Position")]
    [SerializeField] private Transform handTransform;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    private PlayerInputActions inputActions;
    private Camera mainCamera;
    private AudioSource audioSource;
    private InteractableItem currentHoveredItem;
    private InteractionPoint currentHoveredPoint;
    private ItemType currentHeldItem = ItemType.None;
    private GameObject currentHandModel;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnInteract;
    }

    private void Update()
    {
        UpdateHover();
    }

    private void UpdateHover()
    {
        if (virtualCursor == null) return;

        Vector2 screenPosition = virtualCursor.GetCursorPosition();
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, interactableMask))
        {
            if (TryHoverItem(hit)) return;
            if (TryHoverInteractionPoint(hit)) return;
        }

        ClearHover();
    }

    private bool TryHoverItem(RaycastHit hit)
    {
        InteractableItem item = hit.collider.GetComponent<InteractableItem>();

        if (item != null && item.CanInteract())
        {
            if (currentHoveredItem != item)
            {
                ClearHover();
                currentHoveredItem = item;
                currentHoveredItem.SetHighlight(true);
            }
            return true;
        }

        return false;
    }

    private bool TryHoverInteractionPoint(RaycastHit hit)
    {
        InteractionPoint point = hit.collider.GetComponent<InteractionPoint>();

        if (point != null && point.CanInteract())
        {
            if (currentHoveredPoint != point)
            {
                ClearHover();
                currentHoveredPoint = point;
                currentHoveredPoint.SetHighlight(true);
            }
            return true;
        }

        return false;
    }

    private void ClearHover()
    {
        currentHoveredItem?.SetHighlight(false);
        currentHoveredItem = null;

        currentHoveredPoint?.SetHighlight(false);
        currentHoveredPoint = null;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (TryInteractWithItem()) return;
        if (TryInteractWithPoint()) return;
    }

    private bool TryInteractWithItem()
    {
        if (currentHoveredItem != null && currentHoveredItem.CanInteract())
        {
            currentHoveredItem.Interact();
            currentHoveredItem = null;
            return true;
        }

        return false;
    }

    private bool TryInteractWithPoint()
    {
        if (currentHoveredPoint != null && currentHoveredPoint.CanInteract())
        {
            currentHoveredPoint.Interact(currentHeldItem);
            currentHoveredPoint = null;
            return true;
        }

        return false;
    }

    /// <summary>
    /// InteractableItem tarafýndan çaðrýlýr - Item'ý ele al
    /// </summary>
    public void PickupItem(ItemType itemType, GameObject handModel)
    {
        currentHeldItem = itemType;
        PlayPickupSound();
        UpdateHandModel(handModel);
    }

    /// <summary>
    /// InteractionPoint tarafýndan çaðrýlýr - Elindeki item'ý kullan
    /// </summary>
    public void UseItem()
    {
        ClearHandModel();
        currentHeldItem = ItemType.None;
    }

    /// <summary>
    /// LevelManager tarafýndan çaðrýlýr - Inventory temizle
    /// </summary>
    public void ClearInventory()
    {
        currentHeldItem = ItemType.None;
        ClearHandModel();
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }

    private void UpdateHandModel(GameObject handModel)
    {
        if (handModel == null || handTransform == null) return;

        ClearHandModel();

        currentHandModel = Instantiate(handModel, handTransform);
        currentHandModel.transform.localPosition = Vector3.zero;
        currentHandModel.transform.localRotation = Quaternion.identity;
    }

    private void ClearHandModel()
    {
        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
            currentHandModel = null;
        }
    }

    public ItemType GetHeldItem() => currentHeldItem;

    public bool HasItem(ItemType itemType) => currentHeldItem == itemType;

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}