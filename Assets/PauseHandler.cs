using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuCanvasGO;

    // Use a generic Behaviour so you can assign any controller script (PlayerMovement, ThirdPersonController, etc.)
    [SerializeField] private Behaviour _playerController;

    private bool isPaused;

    private void Start()
    {
        if (_mainMenuCanvasGO != null)
            _mainMenuCanvasGO.SetActive(false);
    }

    private void Update()
    {
        // Safe null-check for your InputManager singleton
        if (InputManager.instance == null)
            return;

        if (InputManager.instance.MenuOpenCloseInput)
        {
            if (isPaused)
                Unpause();
            else
                Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (_playerController != null)
            _playerController.enabled = false;

        OpenMainMenu();
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (_playerController != null)
            _playerController.enabled = true;

        CloseAllMenus();
    }

    private void OpenMainMenu()
    {
        if (_mainMenuCanvasGO != null)
            _mainMenuCanvasGO.SetActive(true);
    }

    private void CloseAllMenus()
    {
        if (_mainMenuCanvasGO != null)
            _mainMenuCanvasGO.SetActive(false);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("upgradedMainMenu");
    }

    public void OnResumePressed()
    {
        Unpause();
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}