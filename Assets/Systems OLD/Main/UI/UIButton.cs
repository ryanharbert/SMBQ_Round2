using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public ButtonType buttonType;
	public GameObject popup;
	public string sceneName;

	public delegate void ButtonFunction();
	public ButtonFunction buttonFunction;

	public Image image;

	private void Awake()
	{
		image = GetComponent<Image>();
	}

	public void OnPointerDown (PointerEventData data)
	{
		image.rectTransform.sizeDelta = new Vector2(image.rectTransform.rect.width * 0.9f, image.rectTransform.rect.height * 0.9f);
	}

	public void OnPointerUp (PointerEventData data)
	{
		image.rectTransform.sizeDelta = new Vector2(image.rectTransform.rect.width * (10f/9f), image.rectTransform.rect.height * (10f/9f));

		if(buttonType == ButtonType.LoadScene)
		{
			image.color = Color.red;
			SceneManager.LoadScene(sceneName);
		}
		else if(buttonType == ButtonType.ClosePopUp)
		{
			popup.SetActive(false);
		}
		else if(buttonType == ButtonType.GivenFunction)
		{
			if(buttonFunction != null)
			{
				buttonFunction();
			}
			if(popup != null)
			{
				popup.SetActive(false);
			}
		}
	}
}
