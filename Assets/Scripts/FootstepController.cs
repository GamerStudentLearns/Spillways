using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepController : MonoBehaviour
{
    public AudioClip footstepClip;       // Assign your footstep sound in the Inspector
    public float stepInterval = 0.5f;    // Time between footsteps while moving

    private AudioSource audioSource;
    private CharacterController characterController;
    private float stepTimer = 0f;
    private bool grounded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Check if player is moving
        bool isMoving = characterController != null && characterController.velocity.magnitude > 0.1f;

        // Only play footsteps if moving and grounded
        if (isMoving && grounded)
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
            stepTimer = 0f; // reset timer when not moving
        }
    }

    private void PlayFootstep()
    {
        if (footstepClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }

    // Detect collision with ground
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}

