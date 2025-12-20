using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector3 cameraPosition = new Vector3(0, 5, -10);
    [SerializeField] private Vector3 cameraRotation = new Vector3(20, 0, 0);
    [SerializeField] private bool followPlayer = false;

    [Header("Follow Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -10);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool followXAxis = true;
    [SerializeField] private bool followYAxis = false;
    [SerializeField] private bool followZAxis = false;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (playerTransform == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        // Baþlangýç pozisyonu
        if (!followPlayer)
        {
            cameraTransform.position = cameraPosition;
            cameraTransform.rotation = Quaternion.Euler(cameraRotation);
        }
    }

    private void LateUpdate()
    {
        if (followPlayer && playerTransform != null)
        {
            FollowPlayerSmooth();
        }
    }

    private void FollowPlayerSmooth()
    {
        Vector3 targetPosition = cameraTransform.position;

        if (followXAxis)
        {
            targetPosition.x = playerTransform.position.x + offset.x;
        }

        if (followYAxis)
        {
            targetPosition.y = playerTransform.position.y + offset.y;
        }

        if (followZAxis)
        {
            targetPosition.z = playerTransform.position.z + offset.z;
        }

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }

    public void SetCameraPosition(Vector3 position, Vector3 rotation)
    {
        cameraPosition = position;
        cameraRotation = rotation;

        if (cameraTransform != null)
        {
            cameraTransform.position = position;
            cameraTransform.rotation = Quaternion.Euler(rotation);
        }
    }

    public void EnableFollow(bool enable)
    {
        followPlayer = enable;
    }
}