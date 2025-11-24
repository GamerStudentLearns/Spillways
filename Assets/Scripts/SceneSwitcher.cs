using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Name of the scene you want to load
    public string sceneToLoad;

    // Time delay before switching scenes (editable in Inspector)
    public float delayTime = 3f;

    void Start()
    {
        // Call the function after the chosen delay
        Invoke("LoadNextScene", delayTime);
    }

    void LoadNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);

        // Unlock and show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
