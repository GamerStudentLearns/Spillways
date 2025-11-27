using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    // Assign an AudioClip in the Inspector
    public AudioClip triggerSound;

    private AudioSource audioSource;

    void Start()
    {
        // Get or add an AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            if (triggerSound != null)
            {
                audioSource.PlayOneShot(triggerSound);
            }
        }
    }
}
