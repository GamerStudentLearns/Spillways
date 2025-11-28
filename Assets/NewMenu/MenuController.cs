using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;


public class MenuController : MonoBehaviour
{
    [Header("Levels To Load")]
    public string newGameLevel;
    private string levelToLoad;

    [Header("Audio Settings")]
    [SerializeField] private TMP_Text volumeTextValue = null;
    [SerializeField] private Slider volumeSlider = null;
    [SerializeField] private float defaultVolume = 1.0f;

    [SerializeField] private GameObject comfirmationPrompt = null;

    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text KeyboardSensitivityTextValue = null;
    [SerializeField] private Slider KeyboardSensitivitySlider = null;
    [SerializeField] private int defaultSen = 4;
    public int mainKeyboardSensitivity = 4;

    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertYToggle = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1f;

    [Space(10)]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [SerializeField] private Volume globalVolume;


    private void Start()
    {
        LoadAudioSettings();

        LoadAudioSettings();
        LoadGraphicsSettings(); // IMPORTANT!

        // removed AutoExposure handling (URP/HDRP exposure override no longer controlled here)
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        if (volumeTextValue != null)
            volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void SetKeyboardSensitivity(float sensitivity)
    {
        mainKeyboardSensitivity = Mathf.RoundToInt(sensitivity);
        if (KeyboardSensitivityTextValue != null)
            KeyboardSensitivityTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle != null)
        {
            if (invertYToggle.isOn)
                PlayerPrefs.SetInt("masterInvertY", 1);
            else
                PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainKeyboardSensitivity);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        if (brightnessTextValue != null)
            brightnessTextValue.text = brightness.ToString("0.0");
        // Exposure override removed — brightness value is saved/applied via GraphicsApply
    }


    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityIndex)
    {
        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullScreen", _isFullScreen ? 1 : 0);
        Screen.fullScreen = _isFullScreen;

        StartCoroutine(ComfirmationBox(1f));

    }
    public void NewGameDialogYes()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(newGameLevel);
    }

    public void ResetButton(string MenuType)
    {
        if (MenuType == "Graphics")
        {
            if (brightnessSlider != null)
            {
                brightnessSlider.value = defaultBrightness;
                brightnessTextValue.text = defaultBrightness.ToString("0.0");
            }

            if (qualityDropdown != null)
            {
                qualityDropdown.value = 2;
                QualitySettings.SetQualityLevel(2);
            }

            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = false;
                Screen.fullScreen = false;
            }

            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            if (volumeSlider != null)
                volumeSlider.value = defaultVolume;
            if (volumeTextValue != null)
                volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            if (KeyboardSensitivityTextValue != null)
                KeyboardSensitivityTextValue.text = defaultSen.ToString("0");
            if (KeyboardSensitivitySlider != null)
                KeyboardSensitivitySlider.value = defaultSen;
            mainKeyboardSensitivity = defaultSen;
            if (invertYToggle != null)
                invertYToggle.isOn = false;
            GameplayApply();

        }
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float volume = PlayerPrefs.GetFloat("masterVolume");
            AudioListener.volume = volume;

            if (volumeSlider != null)
                volumeSlider.value = volume;
            if (volumeTextValue != null)
                volumeTextValue.text = volume.ToString("0.0");
        }
    }

    private void LoadGraphicsSettings()
    {
        // BRIGHTNESS
        if (PlayerPrefs.HasKey("masterBrightness"))
        {
            float brightness = PlayerPrefs.GetFloat("masterBrightness");
            _brightnessLevel = brightness;

            if (brightnessSlider != null)
                brightnessSlider.value = brightness;
            if (brightnessTextValue != null)
                brightnessTextValue.text = brightness.ToString("0.0");

            // removed ApplyBrightness call — no direct exposure override control here
        }


        // QUALITY
        if (PlayerPrefs.HasKey("masterQuality"))
        {
            int quality = PlayerPrefs.GetInt("masterQuality");
            _qualityLevel = quality;
            if (qualityDropdown != null)
                qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }

        // FULLSCREEN
        if (PlayerPrefs.HasKey("masterFullScreen"))
        {
            bool full = PlayerPrefs.GetInt("masterFullScreen") == 1;
            _isFullScreen = full;
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = full;
            Screen.fullScreen = full;
        }
    }

    public IEnumerator ComfirmationBox(float duration)
    {
        if (comfirmationPrompt != null)
        {
            comfirmationPrompt.SetActive(true);
            yield return new WaitForSeconds(duration);
            comfirmationPrompt.SetActive(false);
        }
    }
}
