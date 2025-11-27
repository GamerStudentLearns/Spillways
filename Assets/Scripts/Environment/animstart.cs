using UnityEngine;

public class TriggerSingleAnimationOnce : MonoBehaviour
{
    // Assign the Animator in the Inspector
    public Animator animator;

    // Name of the trigger parameter in the Animator
    public string animationTrigger = "Play";

    // Internal flag to ensure it only plays once
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the player and hasn't played yet
        if (!hasPlayed && other.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetTrigger(animationTrigger);
                hasPlayed = true; // prevent re-triggering
            }
        }
    }
}
