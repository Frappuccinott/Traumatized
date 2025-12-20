using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [Header("Transition Settings")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private float transitionDistance = 2f;
    [SerializeField] private bool isLevelExit = true;

    [Header("Visual")]
    [SerializeField] private GameObject transitionPrompt;

    private Transform playerTransform;
    private bool playerInRange = false;
    private bool hasTransitioned = false;

    private void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerController>()?.transform;

        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (hasTransitioned || playerTransform == null) return;

        Vector3 checkPoint = exitPoint != null ? exitPoint.position : transform.position;
        float distance = Vector3.Distance(checkPoint, playerTransform.position);
        bool inRange = distance <= transitionDistance;

        if (inRange != playerInRange)
        {
            playerInRange = inRange;

            if (transitionPrompt != null)
            {
                transitionPrompt.SetActive(playerInRange);
            }

            if (playerInRange && isLevelExit)
            {
                TriggerTransition();
            }
        }
    }

    private void TriggerTransition()
    {
        if (hasTransitioned) return;
        hasTransitioned = true;

        if (transitionPrompt != null)
        {
            transitionPrompt.SetActive(false);
        }

        // Sonraki level'a geç
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadNextLevel();
        }
        else
        {
            Debug.LogWarning("LevelManager not found!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 checkPoint = exitPoint != null ? exitPoint.position : transform.position;
        Gizmos.color = isLevelExit ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPoint, transitionDistance);
    }
}