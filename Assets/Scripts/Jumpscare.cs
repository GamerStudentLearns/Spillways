using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    [Tooltip("The model or object to show during the jumpscare.")]
    public GameObject scareObject;

    [Tooltip("How long the object stays visible.")]
    public float scareDuration = 2f;

    [Tooltip("Optional audio clip for the jumpscare.")]
    public AudioSource scareSound;

    private bool isScaring = false;

    void Start()
    {
        // Ensure the scare object starts hidden
        if (scareObject != null)
        {
            scareObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only trigger if the player enters
        if (other.CompareTag("Player") && !isScaring)
        {
            StartCoroutine(DoJumpscare());
        }
    }

    private System.Collections.IEnumerator DoJumpscare()
    {
        isScaring = true;

        // Show the object
        if (scareObject != null)
        {
            scareObject.SetActive(true);
        }

        // Play sound if assigned
        if (scareSound != null)
        {
            scareSound.Play();
        }

        // Wait for the duration
        yield return new WaitForSeconds(scareDuration);

        // Hide the object again
        if (scareObject != null)
        {
            scareObject.SetActive(false);
        }

        isScaring = false;
    }
}

