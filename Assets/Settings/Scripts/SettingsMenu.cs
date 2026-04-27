using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [SerializeField] Slider masterSlider;
    [SerializeField] Slider SFXSlider;
    [SerializeField] Slider BGMSlider;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + System.Math.Round(resolutions[i].refreshRateRatio.value) + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height &&
                resolutions[i].refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        masterSlider.value = PlayerPrefs.GetFloat("Master", 1f);
        SFXSlider.value = PlayerPrefs.GetFloat("SFX", 1f);
        BGMSlider.value = PlayerPrefs.GetFloat("BGM", 1f);

        masterSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("Master", v));
        SFXSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("SFX", v));
        BGMSlider.onValueChanged.AddListener(v => AudioManager.Instance.SetVolume("BGM", v));
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode, resolution.refreshRateRatio);
    }
    public void SetFullScreen(bool isFullScreen )
    {
        Screen.fullScreenMode = isFullScreen? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}