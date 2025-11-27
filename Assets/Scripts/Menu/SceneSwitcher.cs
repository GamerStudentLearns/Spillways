using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Name of the scene you want to load
    public string sceneToLoad;
    public string UnlockCursorScene;

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

        // Only unlock and show the cursor if the scene is MainMenu
        if (sceneToLoad == UnlockCursorScene)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
