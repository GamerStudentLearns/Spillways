using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootstepController : MonoBehaviour
{
    public AudioClip[] defaultFootsteps;   // Assign in Inspector
    public AudioClip[] waterFootsteps;     // Assign in Inspector
    public AudioClip[] metalFootsteps;     // Assign in Inspector

    // Different delays for each type
    public float defaultStepInterval = 0.5f;
    public float waterStepInterval = 0.8f;
    public float metalStepInterval = 0.3f;

    [Header("Controller settings")]
    [Tooltip("Deadzone for analog stick movement detection (0..1).")]
    public float controllerDeadzone = 0.2f;

    private AudioSource audioSource;
    private CharacterController characterController;
    private float stepTimer = 0f;

    private AudioClip[] currentFootsteps;
    private float currentStepInterval;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();

        // Start with default footsteps
        currentFootsteps = defaultFootsteps;
        currentStepInterval = defaultStepInterval;
    }

    void Update()
    {
        bool isMovingKeyPressed =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        // Legacy axes (works with old Input Manager and many gamepads)
        float axisH = 0f;
        float axisV = 0f;
        try
        {
            axisH = Input.GetAxisRaw("Horizontal");
            axisV = Input.GetAxisRaw("Vertical");
        }
        catch { axisH = 0f; axisV = 0f; }

        bool isMovingAxis = Mathf.Abs(axisH) > controllerDeadzone || Mathf.Abs(axisV) > controllerDeadzone;

        // New Input System gamepad left stick check (if available)
        var gp = Gamepad.current;
        if (gp != null)
        {
            Vector2 left = gp.leftStick.ReadValue();
            if (left.sqrMagnitude > (controllerDeadzone * controllerDeadzone))
                isMovingAxis = true;
        }

        bool isMoving = (isMovingKeyPressed || isMovingAxis) && characterController.isGrounded;

        if (isMoving)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = currentStepInterval; // use the current interval
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (currentFootsteps != null && currentFootsteps.Length > 0)
        {
            int index = Random.Range(0, currentFootsteps.Length);
            audioSource.PlayOneShot(currentFootsteps[index]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WaterZone"))
        {
            currentFootsteps = waterFootsteps;
            currentStepInterval = waterStepInterval;
        }
        else if (other.CompareTag("MetalZone"))
        {
            currentFootsteps = metalFootsteps;
            currentStepInterval = metalStepInterval;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("WaterZone") || other.CompareTag("MetalZone"))
        {
            currentFootsteps = defaultFootsteps;
            currentStepInterval = defaultStepInterval;
        }
    }
}

