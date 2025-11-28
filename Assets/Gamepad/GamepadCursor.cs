using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class GamepadCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private float padding = 20f;

    private bool previousMouseState;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera mainCamera;

    private string previousControlScheme = "";
    private const string gamepadScheme = "Gamepad";
    private const string mouseScheme = "Keyboard&Mouse";

    private void OnEnable()
    {
        mainCamera = Camera.main;
        // use the actual Mouse device, not the string
        currentMouse = Mouse.current;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        if (playerInput != null && virtualMouse != null)
            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if (cursorTransform != null && virtualMouse != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        if (playerInput != null)
            playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        if (virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);

        InputSystem.onAfterUpdate -= UpdateMotion;
        if (playerInput != null)
            playerInput.onControlsChanged -= OnControlsChanged;
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        // Use the right stick for cursor movement instead of the left stick.
        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();
        deltaValue *= cursorSpeed * Time.deltaTime;

        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        InputState.Change(virtualMouse.position, newPosition);
        InputState.Change(virtualMouse.delta, deltaValue);

        // use the local variable for the button state and the correct property name
        bool southButtonPressed = Gamepad.current.buttonSouth != null && Gamepad.current.buttonSouth.isPressed;
        if (previousMouseState != southButtonPressed)
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState = mouseState.WithButton(MouseButton.Left, southButtonPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = southButtonPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 Position)
    {
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform,
            Position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    private void OnControlsChanged(PlayerInput input)
    {
        // guard
        if (playerInput == null) return;

        if (playerInput.currentControlScheme == mouseScheme && previousControlScheme != mouseScheme)
        {
            if (cursorTransform != null) cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            if (currentMouse != null && virtualMouse != null)
                currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previousControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previousControlScheme != gamepadScheme)
        {
            if (cursorTransform != null) cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            if (currentMouse != null && virtualMouse != null)
            {
                InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
                AnchorCursor(currentMouse.position.ReadValue());
            }
            previousControlScheme = gamepadScheme;
        }
    }

    private void Update()
    {
        if (playerInput == null) return;

        if (previousControlScheme != playerInput.currentControlScheme)
        {
            // call with the PlayerInput instance to match signature
            OnControlsChanged(playerInput);
        }
        previousControlScheme = playerInput.currentControlScheme;
    }
}
