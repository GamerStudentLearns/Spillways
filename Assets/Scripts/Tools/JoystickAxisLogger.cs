using UnityEngine;

public class JoystickAxisLogger : MonoBehaviour
{
    public string axisName = "RightStickHorizontal";
    public KeyCode toggleLogKey = KeyCode.None;
    public bool alwaysLog = false;

    void Update()
    {
        if (!alwaysLog && toggleLogKey != KeyCode.None && !Input.GetKey(toggleLogKey))
            return;

        float v = 0f;
        try
        {
            v = Input.GetAxis(axisName);
        }
        catch
        {
            Debug.LogWarning($"Axis '{axisName}' not found in Input Manager.");
            enabled = false;
            return;
        }

        if (Mathf.Abs(v) > 0.01f)
            Debug.Log($"{axisName} = {v:F3}");
    }
}