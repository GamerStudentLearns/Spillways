using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; 

public class Jumpscare : MonoBehaviour
{
    public AudioSource Scream;
    public GameObject Player;
    public GameObject JumpscareCam;

    private bool hasTriggered = false; 

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return; 

        hasTriggered = true; 

        Scream.Play();
        JumpscareCam.SetActive(true);
        Player.SetActive(false);
        StartCoroutine(EndJump());
    }

    IEnumerator EndJump()
    {
        yield return new WaitForSeconds(0.45f);

        
        SceneManager.LoadScene("Credits");
    }
}


