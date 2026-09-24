using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 4.0f;
    public float gravity = -9.81f;

    [Header("Настройки камеры")]
    public Transform playerCamera;
    public float mouseSensitivity = 0.1f;
    public float lookXLimit = 85.0f;

    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Блокируем и скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void Update()
    {
        // 1. Поворот камеры и игрока мышью
        rotationX -= lookInput.y * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }

        transform.rotation *= Quaternion.Euler(0, lookInput.x * mouseSensitivity, 0);

        // 2. Движение на WASD
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized * moveSpeed;

        // 3. Гравитация
        if (!characterController.isGrounded)
        {
            movementDirectionY += gravity * Time.deltaTime;
        }
        else
        {
            movementDirectionY = -0.5f;
        }

        moveDirection.y = movementDirectionY;

        // 4. Итоговый шаг
        characterController.Move(moveDirection * Time.deltaTime);
    }
}