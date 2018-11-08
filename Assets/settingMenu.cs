using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI ;

public class settingMenu : MonoBehaviour {

    public AudioMixer audioMixer;
    public Resolution[] resolutions;
    public Dropdown resDropDown;

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("musicVolume", volume);
    }

    public void SetQulity(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void ActivateFullScreen(bool fullScreenOn)
    {
        Screen.fullScreen = fullScreenOn;
    }

     void Start()
    {
       resolutions = Screen.resolutions;

        resDropDown.ClearOptions();

        List<string> options = new List<string>();
        int currentRes = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentRes = i;
            }
        }

        resDropDown.AddOptions(options);
        resDropDown.value = currentRes;
        resDropDown.RefreshShownValue();
    }
    


}
