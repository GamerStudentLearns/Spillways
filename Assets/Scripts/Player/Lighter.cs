using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public GameObject lighter;
    public GameObject flames;

    public AudioSource lighterSound;

    public bool isOn;

    [Header("Right trigger settings (gamepad)")]
    [Tooltip("Name of the Input Manager axis for the right trigger (create if missing).")]
    public string rightTriggerAxis = "RightTrigger";
    [Tooltip("Fallback joystick button index for triggers (some setups expose triggers as buttons).")]
    public int rightTriggerButtonIndex = 7;
    [Tooltip("Threshold (0..1) to consider the analog trigger pressed.")]
    public float triggerThreshold = 0.5f;

    // internal state for rising-edge detection on analog trigger
    private bool prevTriggerPressed = false;

    void Awake()
    {
        // Force lighter off early so other scripts can't leave it on at start.
        isOn = false;
        if (flames != null)
            flames.SetActive(false);

        if (lighterSound != null)
            lighterSound.Stop();

        // Prevent immediate toggle if trigger / button is already held at startup
        float raw = 0f;
        try
        {
            raw = Input.GetAxisRaw(rightTriggerAxis);
        }
        catch { raw = 0f; }

        float value01 = Mathf.Clamp01((raw + 1f) * 0.5f);
        prevTriggerPressed = (raw >= triggerThreshold || value01 >= triggerThreshold);

        KeyCode triggerButton = (KeyCode)((int)KeyCode.JoystickButton0 + Mathf.Clamp(rightTriggerButtonIndex, 0, 19));
        if (Input.GetKey(triggerButton))
            prevTriggerPressed = true;
    }

    void Start()
    {
        // Keep Start behavior compatible (redundant safety).
        isOn = false;
        if (flames != null) flames.SetActive(false);
    }

    void Update()
    {
        if (lighter == null || flames == null)
            return;

        if (!lighter.activeInHierarchy)
            return;

        // Mouse: left click toggles lighter
        if (Input.GetMouseButtonDown(0))
        {
            ToggleLighter();
            return;
        }

        // Gamepad: check right trigger (axis), with a button fallback
        bool triggerPressedNow = false;

        // Axis check (wrap in try/catch in case axis missing)
        float raw = 0f;
        try
        {
            raw = Input.GetAxisRaw(rightTriggerAxis);
        }
        catch
        {
            raw = 0f;
        }

        // Some drivers report triggers as -1..1, others 0..1 — normalize and test both
        float value01 = Mathf.Clamp01((raw + 1f) * 0.5f);

        if (raw >= triggerThreshold || value01 >= triggerThreshold)
            triggerPressedNow = true;

        // Button fallback (instant press)
        KeyCode triggerButton = (KeyCode)((int)KeyCode.JoystickButton0 + Mathf.Clamp(rightTriggerButtonIndex, 0, 19));
        if (Input.GetKeyDown(triggerButton))
        {
            ToggleLighter();
            prevTriggerPressed = true; // avoid double-fire from axis this frame
            return;
        }

        // Rising-edge detection for axis press
        if (triggerPressedNow && !prevTriggerPressed)
        {
            ToggleLighter();
        }

        prevTriggerPressed = triggerPressedNow;
    }

    void ToggleLighter()
    {
        if (!isOn)
        {
            flames.SetActive(true);
            if (lighterSound != null) lighterSound.Play();
            isOn = true;
        }
        else
        {
            flames.SetActive(false);
            isOn = false;
        }
    }
}
