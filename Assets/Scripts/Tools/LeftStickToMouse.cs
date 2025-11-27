using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class LeftStickToMouse : MonoBehaviour
{
    [Tooltip("Pixels per second the cursor moves when stick is fully deflected.")]
    public float cursorSpeed = 1200f;

    [Tooltip("If true, left stick moves mouse even when cursor is locked.")]
    public bool allowWhenLocked = false;

    void Update()
    {
        var gp = Gamepad.current;
        var mouse = Mouse.current;
        if (gp == null || mouse == null)
            return;

        if (!allowWhenLocked && Cursor.lockState != CursorLockMode.None)
            return;

        Vector2 stick = gp.leftStick.ReadValue(); // -1..1
        if (stick.sqrMagnitude < 0.001f)
            return;

        // delta in pixels this frame
        Vector2 delta = stick * cursorSpeed * Time.unscaledDeltaTime;

        // current mouse pos (screen coordinates)
        Vector2 pos = mouse.position.ReadValue();
        pos += delta;

        // clamp to screen
        pos.x = Mathf.Clamp(pos.x, 0f, (float)Screen.width);
        pos.y = Mathf.Clamp(pos.y, 0f, (float)Screen.height);

        // move the virtual mouse / cursor
        mouse.WarpCursorPosition(pos);

        // also update state so code that reads Mouse.current.position sees change immediately
        // InputState is in UnityEngine.InputSystem.LowLevel
        InputState.Change(mouse.position, pos);
    }
}