using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Name of the scene you want to load
    public string sceneToLoad;

    void Start()
    {
        // Call the function after 3 seconds
        Invoke("LoadNextScene", 3f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
