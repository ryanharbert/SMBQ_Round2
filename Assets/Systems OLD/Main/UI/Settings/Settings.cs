using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;

	public Toggle high;
	public Toggle medium;
	public Toggle low;

    void OnEnable()
    {
        string music = PlayerPrefs.GetString("Music");
        string sfx = PlayerPrefs.GetString("SFX");

        if (music == "on")
        {
            musicToggle.isOn = true;
        }
        else
        {
            musicToggle.isOn = false;
        }

        if (sfx == "on")
        {
            sfxToggle.isOn = true;
        }
        else
        {
            sfxToggle.isOn = false;
        }

		string quality = PlayerPrefs.GetString("Quality");

		if(quality == "high")
		{
			high.isOn = true;
			medium.isOn = false;
			low.isOn = false;
		}
		else if(quality == "low")
		{
			high.isOn = false;
			medium.isOn = false;
			low.isOn = true;
		}
		else
		{
			high.isOn = false;
			medium.isOn = true;
			low.isOn = false;
		}
    }

	public void Low(bool on)
	{
		if(on)
		{
			PlayerPrefs.SetString("Quality", "low");
			QualitySettings.SetQualityLevel(0, true);
		}
	}

	public void Medium(bool on)
	{
		if (on)
		{
			PlayerPrefs.SetString("Quality", "medium");
			QualitySettings.SetQualityLevel(1, true);
		}
	}

	public void High(bool on)
	{
		if (on)
		{
			PlayerPrefs.SetString("Quality", "high");
			QualitySettings.SetQualityLevel(2, true);
		}
	}

	public void Music(bool on)
    {
        if(on)
        {
            PlayerPrefs.SetString("Music", "on");
        }
        else
        {
            PlayerPrefs.SetString("Music", "off");
        }

        AudioManager.instance.Setup();
    }

    public void SFX(bool on)
    {
        if (on)
        {
            PlayerPrefs.SetString("SFX", "on");
        }
        else
        {
            PlayerPrefs.SetString("SFX", "off");
        }

        AudioManager.instance.Setup();
    }

	public void FullScreen()
	{
		if(!Screen.fullScreen)
		{
			Screen.fullScreen = true;
		}
		else
		{
			Screen.fullScreen = false;
		}
	}
}
