using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Karakter hareket kontrolü - WASD/Sol Stick ile hareket, Space/A ile zıplama
/// Character Controller kullanır, 2.5D platformer tarzı hareket
/// SADECE YATAY HAREKET (Sağ-Sol)
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Camera mainCamera;
    private Vector3 velocity;
    private bool isGrounded;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;

    public bool CanMove { get; private set; } = true;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        inputActions.Player.Jump.performed -= OnJump;
    }

    private void Update()
    {
        CheckGround();

        if (CanMove)
        {
            Move();
        }

        ApplyGravity();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (CanMove && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void Move()
    {
        if (mainCamera == null) return;

        // SADECE X EKSENİ (Sağ-Sol)
        // moveInput.x → A/D veya Left Stick yatay
        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = right * moveInput.x;

        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        // Hareket varsa karakteri o yöne döndür
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            Vector3 lookDirection = moveInput.x > 0 ? right : -right;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void DisableMovement()
    {
        CanMove = false;
        moveInput = Vector2.zero;
    }

    public void EnableMovement()
    {
        CanMove = true;
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }
}