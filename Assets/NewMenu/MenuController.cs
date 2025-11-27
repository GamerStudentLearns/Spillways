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

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;

    [SerializeField] private Volume globalVolume;


    private void Start()
    {
        LoadAudioSettings();
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        LoadAudioSettings();
        LoadGraphicsSettings(); // IMPORTANT!

        // removed AutoExposure handling (URP/HDRP exposure override no longer controlled here)

        // your resolution setup code...
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
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
        KeyboardSensitivityTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainKeyboardSensitivity);
        StartCoroutine(ComfirmationBox(1f));
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
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
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 2;
            QualitySettings.SetQualityLevel(2);

            fullscreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;
            GraphicsApply();
        }

        if (MenuType == "Audio")
        {
            AudioListener.volume = defaultVolume;
            volumeSlider.value = defaultVolume;
            volumeTextValue.text = defaultVolume.ToString("0.0");
            VolumeApply();
        }

        if (MenuType == "Gameplay")
        {
            KeyboardSensitivityTextValue.text = defaultSen.ToString("0");
            KeyboardSensitivitySlider.value = defaultSen;
            mainKeyboardSensitivity = defaultSen;
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

            volumeSlider.value = volume;
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

            brightnessSlider.value = brightness;
            brightnessTextValue.text = brightness.ToString("0.0");

            // removed ApplyBrightness call — no direct exposure override control here
        }


        // QUALITY
        if (PlayerPrefs.HasKey("masterQuality"))
        {
            int quality = PlayerPrefs.GetInt("masterQuality");
            _qualityLevel = quality;
            qualityDropdown.value = quality;
            QualitySettings.SetQualityLevel(quality);
        }

        // FULLSCREEN
        if (PlayerPrefs.HasKey("masterFullScreen"))
        {
            bool full = PlayerPrefs.GetInt("masterFullScreen") == 1;
            _isFullScreen = full;
            fullscreenToggle.isOn = full;
            Screen.fullScreen = full;
        }
    }

    public IEnumerator ComfirmationBox(float duration)
    {
        comfirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(duration);
        comfirmationPrompt.SetActive(false);
    }
}
