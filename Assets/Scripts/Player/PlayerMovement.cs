using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    [Header("Zone Speed Multipliers")]
    public float waterSpeedMultiplier = 0.5f; // 50% slower in water

    [Header("Look")]
    public float mouseSensitivity = 0.15f; // multiplier for mouse delta
    public float gamepadSensitivity = 150f; // degrees/sec per stick unit
    public bool invertY = false;

    // Input System actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    private float baseSpeed;       // original speed
    private float currentSpeed;    // speed that changes depending on zone

    void Awake()
    {
        controller = GetComponent<CharacterController>();

        // Move: gamepad left stick + WASD composite
        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("2DVector")
                  .With("Up", "<Keyboard>/w")
                  .With("Down", "<Keyboard>/s")
                  .With("Left", "<Keyboard>/a")
                  .With("Right", "<Keyboard>/d");

        // Look: mouse delta + gamepad right stick
        lookAction = new InputAction("Look", InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");

        // Jump: spacebar and gamepad south (A / Cross)
        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        baseSpeed = speed;
        currentSpeed = baseSpeed;

        if (cameraTransform != null)
        {
            float e = cameraTransform.localEulerAngles.x;
            if (e > 180f) e -= 360f;
            xRotation = e;
        }
    }

    void Update()
    {
        // Ground check + gravity
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Movement
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Look
        Vector2 rawLook = lookAction.ReadValue<Vector2>();
        bool mouseUsed = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.000001f;

        float deltaX, deltaY;
        if (mouseUsed)
        {
            deltaX = rawLook.x * mouseSensitivity;
            deltaY = rawLook.y * mouseSensitivity;
        }
        else
        {
            deltaX = rawLook.x * gamepadSensitivity * Time.deltaTime;
            deltaY = rawLook.y * gamepadSensitivity * Time.deltaTime;
        }

        if (invertY) deltaY = -deltaY;

        xRotation -= deltaY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * deltaX);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterZone"))
        {
            currentSpeed = baseSpeed * waterSpeedMultiplier;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterZone"))
        {
            currentSpeed = baseSpeed;
        }
    }
}

