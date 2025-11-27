using UnityEngine;

public class TriggerAppearDisappear : MonoBehaviour
{
    // Time in seconds before the object disappears again
    public float visibleDuration = 3f;

    private Renderer objectRenderer;

    void Start()
    {
        // Get the Renderer component
        objectRenderer = GetComponent<Renderer>();

        // Start invisible
        objectRenderer.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Make visible
            objectRenderer.enabled = true;

            // Schedule disappearance
            Invoke("MakeInvisible", visibleDuration);
        }
    }

    void MakeInvisible()
    {
        objectRenderer.enabled = false;
    }
}


