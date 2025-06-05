using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public float maxLookUpAngle = 80f;
    public float maxLookDownAngle = -80f;
    public Transform playerCamera;

    [Header("Pushing Settings")]
    public float pushForce = 5f;
    public float pushMassLimit = 3f;
    public float pushDetectionDistance = 1.2f;
    public float pushSlowdownFactor = 0.5f;
    public float pushAngleThreshold = 45f; // ���� ��� ����������� ��������

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Animator animator;

    [Header("Person Settings")]
    public bool isGrounded;
    public bool isSit;
    public bool UIOpen;
    public bool isPushing;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public LayerMask interactionLayer;
    public KeyCode interactKey = KeyCode.E;

    private IInteractable currentInteractable;
    private bool canInteract = true;
    private PushableObject currentPushable;
    private Vector3 lastMoveDirection;

    private void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    private void Update()
    {
        CheckGrounded();
        Gravity();

        if (UIOpen) return;

        InputHandler();
        HandleLook();
        HandleInteraction();
        UpdatePushingState();
    }

    private void UpdatePushingState()
    {
        // ���������� ��������� ��������, ���� �� ���������
        if (lastMoveDirection.magnitude < 0.1f)
        {
            isPushing = false;
            currentPushable = null;
            return;
        }

        // ��������� ������ ����� �������
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, pushDetectionDistance))
        {
            PushableObject pushable = hit.collider.GetComponent<PushableObject>();
            if (pushable != null && pushable.CanBePushed())
            {
                // ��������� ���� ����� ������������ �������� � ��������
                float angle = Vector3.Angle(lastMoveDirection, hit.normal);
                if (angle > 180f - pushAngleThreshold || angle < pushAngleThreshold)
                {
                    isPushing = true;
                    currentPushable = pushable;
                    return;
                }
            }
        }

        isPushing = false;
        currentPushable = null;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (UIOpen || !canInteract || !isPushing) return;

        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic) return;

        // ���������, ��� ��� ��� �� ������, ������� �� �������
        if (currentPushable != null && hit.collider.gameObject != currentPushable.gameObject) return;

        if (body.mass > pushMassLimit) return;
        if (hit.moveDirection.y < -0.3f) return;

        // ��������� ����������� ������ �� ������ ���������� ����������� ��������
        Vector3 pushDir = new Vector3(lastMoveDirection.x, 0, lastMoveDirection.z).normalized;
        body.velocity = pushDir * pushForce / (body.mass * 0.5f + 1);
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position, characterController.radius + groundCheckDistance, groundMask);
        if (!isGrounded)
        {
            isGrounded = characterController.isGrounded;
        }
    }

    private void InputHandler()
    {
        HandleMovement();
        if (Input.GetButtonDown("Jump") && isGrounded && !isPushing)
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ��������� ��������� ����������� ��������
        lastMoveDirection = transform.right * x + transform.forward * z;

        float currentSpeed = 0;
        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0) && !isPushing)
        {
            currentSpeed = sprintSpeed;
            animator.SetInteger("IsRun", 2);
        }
        else if (x != 0 || z != 0)
        {
            currentSpeed = isPushing ? moveSpeed * pushSlowdownFactor : moveSpeed;
            animator.SetInteger("IsRun", 1);
        }
        else
        {
            animator.SetInteger("IsRun", 0);
        }

        characterController.Move(lastMoveDirection.normalized * currentSpeed * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        if (isPushing) return;

        Ray ray = new Ray(playerCamera.position, playerCamera.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null && interactable != currentInteractable)
            {
                if (currentInteractable != null)
                    currentInteractable.SetHighlight(false);

                currentInteractable = interactable;
                currentInteractable.SetHighlight(true);
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.SetHighlight(false);
            currentInteractable = null;
        }

        if (canInteract && currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact();
            canInteract = false;
            Invoke(nameof(ResetInteraction), 0.5f);
        }
    }

    private void ResetInteraction()
    {
        canInteract = true;
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Gravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}