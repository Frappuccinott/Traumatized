using UnityEngine;
using UnityEngine.InputSystem;

public class QTETrigger : MonoBehaviour
{
    [Header("QTE Settings")]
    [SerializeField] private QTESequence qteSequence;
    [SerializeField] private float interactionRadius = 2f;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject interactionPrompt;

    private PlayerInputActions inputActions;
    private Transform playerTransform;
    private bool playerInRange = false;
    private bool qteCompleted = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Interact.performed += OnInteractPressed;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Interact.performed -= OnInteractPressed;
    }

    private void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerController>()?.transform;
    }

    private void Update()
    {
        if (qteCompleted || playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance <= interactionRadius;

        if (inRange != playerInRange)
        {
            playerInRange = inRange;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(playerInRange);
            }
        }
    }

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (!playerInRange || qteCompleted) return;

        // Item kontrolü
        PlayerInteraction playerInteraction = playerTransform.GetComponent<PlayerInteraction>();
        if (playerInteraction != null && !string.IsNullOrEmpty(qteSequence.RequiredItemID))
        {
            if (!playerInteraction.HasItem(qteSequence.RequiredItemID))
            {
                Debug.Log("Required item not found!");
                return;
            }
        }

        // QTE baþlat
        StartQTE();
    }

    private void StartQTE()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        QTEManager.Instance?.StartQTE(qteSequence, OnQTESuccess, OnQTEFail);
    }

    private void OnQTESuccess()
    {
        qteCompleted = true;
        Debug.Log("QTE Success!");

        LevelManager.Instance?.OnQTESuccess();
    }

    private void OnQTEFail()
    {
        Debug.Log("QTE Failed!");

        LevelManager.Instance?.OnQTEFail();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}