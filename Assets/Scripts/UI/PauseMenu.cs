using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject volumeSettings;
    private bool volumeSettingsActive = false;

    [SerializeField] private Slider sliderBMG;
    [SerializeField] private Slider sliderEnemies;
    [SerializeField] private Slider sliderFX;

    void Start()
    {
        sliderBMG.value = PlayerPrefs.GetFloat("VolumeBGM", 1f);
        sliderEnemies.value = PlayerPrefs.GetFloat("VolumeEnemies", 1f);
        sliderFX.value = PlayerPrefs.GetFloat("VolumeFX", 1f);
    }

    private void OnEnable()
    {
        volumeSettings.SetActive(false);
    }

    public void ToggleVolumeSettings()
    {
        if (volumeSettingsActive)
        {
            volumeSettings.SetActive(false);
            volumeSettingsActive = false;
        }
        else
        {
            volumeSettings.SetActive(true);
            volumeSettingsActive = true;
        }
    }

    public void ChangeBGMVolume()
    {
        PlayerPrefs.SetFloat("VolumeBGM", sliderBMG.value);
        AudioManager.instance.audioSourceBGM.volume = sliderBMG.value;
    }

    public void ChangeEnemyVolume()
    {
        PlayerPrefs.SetFloat("VolumeEnemies", sliderEnemies.value);
        AudioManager.instance.audioSourceSoundFXEnemies.volume = sliderEnemies.value;
    }

    public void ChangeFXVolume()
    {
        PlayerPrefs.SetFloat("VolumeFX", sliderFX.value);
        AudioManager.instance.audioSourceSoundFX.volume = sliderFX.value;
    }
}
