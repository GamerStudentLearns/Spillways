using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        // Check for any "pause" input this frame:
        // - Keyboard Escape (legacy Input)
        // - Keyboard Escape via new Input System
        // - Gamepad Start/Options (new Input System)
        bool pausePressed = false;

        // Legacy check (keeps existing behavior)
        if (Input.GetKeyDown(KeyCode.Escape))
            pausePressed = true;

        // New Input System checks (if package active)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            pausePressed = true;

        var gp = Gamepad.current;
        if (gp != null && gp.startButton.wasPressedThisFrame)
            pausePressed = true;

        if (pausePressed)
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu2");
        Debug.Log("Loading Menu...");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}
