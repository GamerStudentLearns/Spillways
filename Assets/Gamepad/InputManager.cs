using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public bool MenuOpenCloseInput { get; private set; }

    private PlayerInput _playerInput;
    private InputAction _menuOpenCloseAction;

    private void Awake()
    {
        // Simple singleton protection
        if (instance == null)
        {
            instance = this;
            // Optionally: DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Debug.LogWarning("Multiple InputManager instances detected; destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogWarning("InputManager: No PlayerInput component found on the GameObject.");
            return;
        }

        // Find the action safely; will return null if action missing
        _menuOpenCloseAction = _playerInput.actions.FindAction("MenuOpenClose", throwIfNotFound: false);
        if (_menuOpenCloseAction == null)
        {
            Debug.LogWarning("InputManager: Action 'MenuOpenClose' not found in PlayerInput.actions.");
            return;
        }

        // Ensure the action is enabled so .triggered works
        if (!_menuOpenCloseAction.enabled)
            _menuOpenCloseAction.Enable();
    }

    private void Update()
    {
        // Safely poll the action using .triggered
        MenuOpenCloseInput = _menuOpenCloseAction != null && _menuOpenCloseAction.triggered;
    }
}
