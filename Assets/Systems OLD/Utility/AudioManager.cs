using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

	public AudioSource musicSource;

    public AudioSource clickSource;

    public AudioMixerSnapshot normal;
    public AudioMixerSnapshot musicMute;
    public AudioMixerSnapshot sfxMute;
    public AudioMixerSnapshot bothMute;

    float transitionTime = 0.01f;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartMusic()
    {
        Setup();
        Invoke("PlayMusic", 1f);
    }

    void PlayMusic()
    {
        musicSource.Play();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            clickSource.Play();
        }
    }

    public void Setup()
    {
        string music = PlayerPrefs.GetString("Music");
        string sfx = PlayerPrefs.GetString("SFX");

        if (music == "")
        {
            PlayerPrefs.SetString("Music", "on");
            PlayerPrefs.SetString("SFX", "on");
        }
        else if (music == "on" && sfx == "on")
        {
            normal.TransitionTo(transitionTime);
        }
        else if (music == "off" && sfx == "on")
        {
             musicMute.TransitionTo(transitionTime);
        }
        else if (music == "on" && sfx == "off")
        {
            sfxMute.TransitionTo(transitionTime);
        }
        else if (music == "off" && sfx == "off")
        {
            bothMute.TransitionTo(transitionTime);
        }
    }

    public void Mute()
    {
        PlayerPrefs.SetString("Music", "off");
        PlayerPrefs.SetString("SFX", "off");

        Setup();
    }
}
