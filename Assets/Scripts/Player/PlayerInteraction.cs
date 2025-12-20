using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float mouseRaycastRange = 50f;
    [SerializeField] private float gamepadInteractionRadius = 3f;
    [SerializeField] private LayerMask interactableMask;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;

    private PlayerInputActions inputActions;
    private Camera mainCamera;
    private AudioSource audioSource;

    private InteractableItem currentHoveredItem; // Mouse için
    private InteractableItem currentNearestItem; // Gamepad için

    private List<string> collectedItems = new List<string>();

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        CheckMouseHover();
        CheckGamepadNearby();
    }

    private void CheckMouseHover()
    {
        // Mouse kullanýlýyorsa
        if (Mouse.current != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, mouseRaycastRange, interactableMask))
            {
                InteractableItem item = hit.collider.GetComponent<InteractableItem>();
                if (item != null && item.CanInteract())
                {
                    if (currentHoveredItem != item)
                    {
                        // Önceki item'ýn highlight'ýný kapat
                        if (currentHoveredItem != null)
                        {
                            currentHoveredItem.SetHighlight(false);
                        }

                        // Yeni item'ý highlight et
                        currentHoveredItem = item;
                        currentHoveredItem.SetHighlight(true);
                    }
                    return;
                }
            }

            // Hiçbir þeye hover deðilse, highlight'ý kapat
            if (currentHoveredItem != null)
            {
                currentHoveredItem.SetHighlight(false);
                currentHoveredItem = null;
            }
        }
    }

    private void CheckGamepadNearby()
    {
        // Gamepad kullanýlýyorsa
        if (Gamepad.current != null)
        {
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, gamepadInteractionRadius, interactableMask);

            InteractableItem nearestItem = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider col in nearbyColliders)
            {
                InteractableItem item = col.GetComponent<InteractableItem>();
                if (item != null && item.CanInteract())
                {
                    float distance = Vector3.Distance(transform.position, col.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestItem = item;
                    }
                }
            }

            // En yakýn item deðiþtiyse
            if (currentNearestItem != nearestItem)
            {
                if (currentNearestItem != null)
                {
                    currentNearestItem.SetHighlight(false);
                }

                currentNearestItem = nearestItem;

                if (currentNearestItem != null)
                {
                    currentNearestItem.SetHighlight(true);
                }
            }
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractableItem targetItem = null;

        // Mouse kullanýyorsa
        if (currentHoveredItem != null)
        {
            targetItem = currentHoveredItem;
        }
        // Gamepad kullanýyorsa
        else if (currentNearestItem != null)
        {
            targetItem = currentNearestItem;
        }

        if (targetItem != null && targetItem.CanInteract())
        {
            CollectItem(targetItem);
        }
    }

    private void CollectItem(InteractableItem item)
    {
        // Ses çal
        if (pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }

        // Item'ý hafýzaya al
        collectedItems.Add(item.ItemID);

        // Item'ý yok et
        item.OnCollected();

        // Referanslarý temizle
        if (currentHoveredItem == item) currentHoveredItem = null;
        if (currentNearestItem == item) currentNearestItem = null;
    }

    public bool HasItem(string itemID)
    {
        return collectedItems.Contains(itemID);
    }

    public void ClearItems()
    {
        collectedItems.Clear();
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}