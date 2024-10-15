using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsModal : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Video")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Mouse")]
    [SerializeField] private Slider mouseSensitivitySlider;


    public void SaveSettings()
    {
        SaveVolumeSettings();
        SaveVideoSettings();
        SaveMouseSettings();
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("music-volume", musicVolumeSlider.value);
        PlayerPrefs.SetFloat("sfx-volume", sfxVolumeSlider.value);

        AudioController.SetMusicVolume(musicVolumeSlider.value);
        AudioController.SetSfxVolume(sfxVolumeSlider.value);
    }

    private void SaveVideoSettings()
    {
        string[] resolution = resolutionDropdown.captionText.text.Split('x');
        int width = Convert.ToInt32(resolution[0]);
        int height = Convert.ToInt32(resolution[1]);

        UnityEngine.Screen.SetResolution(width, height, fullScreenToggle.isOn);
        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;
    }

    private void SaveMouseSettings()
    {
        MouseLook.sensitivity = mouseSensitivitySlider.value;
        PlayerPrefs.SetFloat("mouse-sensitivity", MouseLook.sensitivity);
    }

    private void LoadResolutions()
    {
        List<string> resolutions = new List<string>();

        for (int i = Screen.resolutions.Length; i --> 0;)
        {
            Resolution resolution = Screen.resolutions[i];
            string resolutionString = $"{resolution.width}x{resolution.height}";

            if (resolutions.Contains(resolutionString))
                continue;

            resolutions.Add(resolutionString);
        }
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutions);

        string currentResolutionString = $"{UnityEngine.Screen.width}x{UnityEngine.Screen.height}";
        resolutionDropdown.value = resolutionDropdown.options
                                       .FindIndex(option => option.text == currentResolutionString);
    }

    public void SetDefaultSettings()
    {
        SetDefaultVolumeSettings();
        SetDefaultVideoSettings();
        SetDefaultMouseSettings();
    }

    private void SetDefaultVolumeSettings()
    {
        musicVolumeSlider.value = 1f;
    }

    private void SetDefaultVideoSettings()
    {
        fullScreenToggle.isOn = true;
        vSyncToggle.isOn = true;
        resolutionDropdown.value = 0;
    }

    private void SetDefaultMouseSettings()
    {
        mouseSensitivitySlider.value = 100f;
    }


    void OnEnable()
    {
        // Music
        musicVolumeSlider.value = PlayerPrefs.GetFloat("music-volume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfx-volume", 1f);

        // Video
        LoadResolutions();
        fullScreenToggle.isOn = UnityEngine.Screen.fullScreen;
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        // Mouse
        mouseSensitivitySlider.value = PlayerPrefs.GetFloat("mouse-sensitivity", 100f);
    }
}
