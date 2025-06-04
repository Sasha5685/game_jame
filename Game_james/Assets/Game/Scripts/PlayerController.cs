using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Look Settings")]
    public float mouseSensitivity = 100f;
    public float maxLookUpAngle = 80f;
    public float maxLookDownAngle = -80f;
    public Transform playerCamera;

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;
    private Animator Animator;
    [Header("Perscon Settings")]
    public bool IsGrounded;
    public bool IsSit;

    private void Start()
    {
        Animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (var renderer in this.gameObject.GetComponentsInChildren<Renderer>())
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
        }
    }

    private void Update()
    {
        IsGrounded = characterController.isGrounded;

        InputHandler();
        HandleLook();
        Gravity();
    }
    private void InputHandler()
    {
        HandleMovement();
        if (Input.GetButtonDown("Jump") && IsGrounded) {Jump();}
    }

    private void HandleMovement()
    {
        // �������� ���� � ����������
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // ��������� ����������� �������� ������������ ���������� ���������
        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = 0;
        if (Input.GetKey(KeyCode.LeftShift) && x != 0 || z != 0)
        {
            currentSpeed = sprintSpeed;
            Animator.SetInteger("IsRun", 2);
        }
        else if(x != 0 ||  z != 0)
        {
            currentSpeed = moveSpeed;
            Animator.SetInteger("IsRun", 1);
        }
        else
        {
            Animator.SetInteger("IsRun", 0);
        }
        // ������� ���������
        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    private void HandleLook()
    {
        // �������� ���� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ������������ ������� (�����/����)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);

        // ��������� ������� ������ �� ���������
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // �������������� ������� (�����/������) - ������������ ���� ������� ������
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Sit()
    {

    }
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Gravity()
    {
        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ��������� ���� ��������� � �����
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
