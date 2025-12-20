//using UnityEngine;
//using UnityEngine.InputSystem;
//using System.Collections.Generic;

///// <summary>
///// Oyuncu etkileþim sistemi - Virtual cursor ile item hover ve toplama
///// Input Actions sistemi ile etkileþim (E tuþu / Gamepad Interact butonu)
///// </summary>
//public class PlayerInteraction : MonoBehaviour
//{
//    [Header("Interaction Settings")]
//    [SerializeField] private float raycastRange = 50f;
//    [SerializeField] private LayerMask interactableMask;

//    [Header("Virtual Cursor")]
//    [SerializeField] private VirtualCursor virtualCursor;

//    [Header("Audio")]
//    [SerializeField] private AudioClip pickupSound;

//    private PlayerInputActions inputActions;
//    private Camera mainCamera;
//    private AudioSource audioSource;
//    private InteractableItem currentHoveredItem;
//    private List<string> inventory = new List<string>();

//    private void Awake()
//    {
//        inputActions = new PlayerInputActions();
//        mainCamera = Camera.main;
//        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
//    }

//    private void OnEnable()
//    {
//        inputActions.Player.Enable();
//        inputActions.Player.Interact.performed += OnInteract;
//    }

//    private void OnDisable()
//    {
//        inputActions.Player.Disable();
//        inputActions.Player.Interact.performed -= OnInteract;
//    }

//    private void Update()
//    {
//        UpdateHover();
//    }

//    /// <summary>
//    /// Virtual cursor pozisyonunda raycast ile item kontrol et
//    /// </summary>
//    private void UpdateHover()
//    {
//        if (virtualCursor == null) return;

//        Vector2 screenPosition = virtualCursor.GetCursorPosition();
//        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

//        if (Physics.Raycast(ray, out RaycastHit hit, raycastRange, interactableMask))
//        {
//            InteractableItem item = hit.collider.GetComponent<InteractableItem>();

//            if (item != null && item.CanInteract())
//            {
//                if (currentHoveredItem != item)
//                {
//                    currentHoveredItem?.SetHighlight(false);
//                    currentHoveredItem = item;
//                    currentHoveredItem.SetHighlight(true);
//                }
//                return;
//            }
//        }

//        // Hover edilen item yoksa highlight kapat
//        if (currentHoveredItem != null)
//        {
//            currentHoveredItem.SetHighlight(false);
//            currentHoveredItem = null;
//        }
//    }

//    /// <summary>
//    /// Input Actions'dan Interact eventi geldiðinde çaðrýlýr
//    /// </summary>
//    private void OnInteract(InputAction.CallbackContext context)
//    {
//        if (currentHoveredItem != null && currentHoveredItem.CanInteract())
//        {
//            currentHoveredItem.Interact();
//            currentHoveredItem = null;
//        }
//    }

//    /// <summary>
//    /// Item'ý inventory'e ekle
//    /// </summary>
//    public void AddItem(string itemID)
//    {
//        inventory.Add(itemID);

//        if (pickupSound != null)
//        {
//            audioSource.PlayOneShot(pickupSound);
//        }
//    }

//    public bool HasItem(string itemID) => inventory.Contains(itemID);

//    public void ClearInventory() => inventory.Clear();

//    private void OnDestroy()
//    {
//        inputActions?.Dispose();
//    }
//}

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Oyuncu etkileþim sistemi - Item toplama ve kullanma
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float raycastRange = 50f;
    [SerializeField] private LayerMask interactableMask;

    [Header("Virtual Cursor")]
    [SerializeField] private VirtualCursor virtualCursor;

    [Header("Hand Position")]
    [SerializeField] private Transform handTransform; // Elle tutulan item'in parent'i

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    private PlayerInputActions inputActions;
    private Camera mainCamera;
    private AudioSource audioSource;
    private InteractableItem currentHoveredItem;
    private InteractionPoint currentHoveredPoint;

    // Inventory
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
            // Item kontrolü
            InteractableItem item = hit.collider.GetComponent<InteractableItem>();
            if (item != null && item.CanInteract())
            {
                if (currentHoveredItem != item)
                {
                    ClearHover();
                    currentHoveredItem = item;
                    currentHoveredItem.SetHighlight(true);
                }
                return;
            }

            // Interaction Point kontrolü
            InteractionPoint point = hit.collider.GetComponent<InteractionPoint>();
            if (point != null && point.CanInteract())
            {
                if (currentHoveredPoint != point)
                {
                    ClearHover();
                    currentHoveredPoint = point;
                    currentHoveredPoint.SetHighlight(true);
                }
                return;
            }
        }

        ClearHover();
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
        // Item topla
        if (currentHoveredItem != null && currentHoveredItem.CanInteract())
        {
            currentHoveredItem.Interact();
            currentHoveredItem = null;
            return;
        }

        // Interaction Point kullan
        if (currentHoveredPoint != null && currentHoveredPoint.CanInteract())
        {
            currentHoveredPoint.Interact(currentHeldItem);
            currentHoveredPoint = null;
            return;
        }
    }

    /// <summary>
    /// Item'ý ele al
    /// </summary>
    public void PickupItem(ItemType itemType, GameObject handModel)
    {
        currentHeldItem = itemType;

        // Ses çal
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Elle tutulan modeli göster
        if (handModel != null && handTransform != null)
        {
            // Önceki modeli temizle
            if (currentHandModel != null)
            {
                Destroy(currentHandModel);
            }

            // Yeni modeli instantiate et
            currentHandModel = Instantiate(handModel, handTransform);
            currentHandModel.transform.localPosition = Vector3.zero;
            currentHandModel.transform.localRotation = Quaternion.identity;
        }

        Debug.Log($"Picked up: {itemType}");
    }

    /// <summary>
    /// Elindeki item'ý kullan (mini game sonrasý)
    /// </summary>
    public void UseItem()
    {
        Debug.Log($"Used: {currentHeldItem}");

        // Elle tutulan modeli temizle
        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
            currentHandModel = null;
        }

        currentHeldItem = ItemType.None;
    }

    public ItemType GetHeldItem() => currentHeldItem;

    public bool HasItem(ItemType itemType) => currentHeldItem == itemType;

    public void ClearInventory()
    {
        currentHeldItem = ItemType.None;

        if (currentHandModel != null)
        {
            Destroy(currentHandModel);
            currentHandModel = null;
        }
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}