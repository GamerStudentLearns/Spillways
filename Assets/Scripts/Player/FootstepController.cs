using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootstepController : MonoBehaviour
{
    public AudioClip footstepClip;       // Assign your footstep sound in the Inspector
    public float stepInterval = 0.5f;    // Time between footsteps

    private AudioSource audioSource;
    private CharacterController characterController;
    private float stepTimer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if any movement key is pressed
        bool isMovingKeyPressed =
            Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D);

        // Only play footsteps if moving keys are pressed AND grounded
        if (isMovingKeyPressed && characterController.isGrounded)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // reset timer when not moving or not grounded
        }
    }

    private void PlayFootstep()
    {
        if (footstepClip != null)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }
}
