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

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Animator animator;

    [Header("Person Settings")]
    public bool isGrounded;
    public bool isSit;
    public bool UIOpen;

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
    }

    private void CheckGrounded()
    {
        // Более точная проверка нахождения на земле
        isGrounded = Physics.CheckSphere(transform.position, characterController.radius + groundCheckDistance, groundMask);

        // Дополнительная проверка через CharacterController
        if (!isGrounded)
        {
            isGrounded = characterController.isGrounded;
        }
    }

    private void InputHandler()
    {
        HandleMovement();
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = 0;
        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0))
        {
            currentSpeed = sprintSpeed;
            animator.SetInteger("IsRun", 2);
        }
        else if (x != 0 || z != 0)
        {
            currentSpeed = moveSpeed;
            animator.SetInteger("IsRun", 1);
        }
        else
        {
            animator.SetInteger("IsRun", 0);
        }

        characterController.Move(move * currentSpeed * Time.deltaTime);
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

    // Визуализация сферы проверки земли в редакторе
    private void OnDrawGizmosSelected()
    {
        if (characterController == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, characterController.radius + groundCheckDistance);
    }
}