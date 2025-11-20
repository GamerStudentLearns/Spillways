using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTriggerLoader : MonoBehaviour
{
    public string sceneToLoad; // Set this in the Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the object has the "Player" tag
        {
            LoadScene(sceneToLoad);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
