using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneAudio : MonoBehaviour {

	public AudioClip musicClip;

	private void Awake()
	{
		if(AudioManager.instance != null && AudioManager.instance.musicSource.clip.name != musicClip.name)
		{
			AudioManager.instance.musicSource.clip = musicClip;
			AudioManager.instance.musicSource.Play();
		}
	}
}
