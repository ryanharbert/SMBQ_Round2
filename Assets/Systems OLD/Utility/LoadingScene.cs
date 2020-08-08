using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviour
{
	public Image image;
	public GameObject loadingObject;

	private void Awake()
	{
		image.color = Color.clear;
		loadingObject.SetActive(false);
	}
}
