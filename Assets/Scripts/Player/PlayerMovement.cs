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

    [Header("Look")]
    public float mouseSensitivity = 0.15f; // multiplier for mouse delta
    public float gamepadSensitivity = 150f; // degrees/sec per stick unit
    public bool invertY = false;

    // Input System actions (created in code for simplicity)
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

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

        // initialize xRotation from current camera transform so we don't jump
        if (cameraTransform != null)
        {
            float e = cameraTransform.localEulerAngles.x;
            if (e > 180f) e -= 360f;
            xRotation = e;
        }
    }

    void Update()
    {
        // Ground check + gravity (same behavior as before)
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        // Movement
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (jumpAction.WasPressedThisFrame() && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravity application
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Look
        Vector2 rawLook = lookAction.ReadValue<Vector2>();

        // Detect if mouse produced the input this frame (mouse delta non-zero)
        bool mouseUsed = Mouse.current != null && Mouse.current.delta.ReadValue().sqrMagnitude > 0.000001f;

        float deltaX;
        float deltaY;
        if (mouseUsed)
        {
            // mouse delta is pixels -> scale by mouseSensitivity and Time.deltaTime
            deltaX = rawLook.x * mouseSensitivity;
            deltaY = rawLook.y * mouseSensitivity;
            // multiply by 1 (not Time.deltaTime) — mouse delta is already framerate independent when using sensitivity as a small scalar
            deltaX *= 1f;
            deltaY *= 1f;
        }
        else
        {
            // gamepad right stick is -1..1 -> treat as degrees/sec then * Time.deltaTime
            deltaX = rawLook.x * gamepadSensitivity * Time.deltaTime;
            deltaY = rawLook.y * gamepadSensitivity * Time.deltaTime;
        }

        // invert if requested
        if (invertY) deltaY = -deltaY;

        // If using mouse, apply per-frame rotation directly; if gamepad, delta is already in degrees and time-scaled
        if (mouseUsed)
        {
            xRotation -= deltaY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * deltaX);
        }
        else
        {
            // gamepad: deltaX/deltaY already include Time.deltaTime
            xRotation -= deltaY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * deltaX);
        }
    }
}
