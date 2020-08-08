using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterShopUI : MonoBehaviour
{
	public Button button;
	public Text buttonHeader;
	public Text buttonAction;
	public GameObject enterShopObject;
	public GameObject stillOnNodeButton;
	public GameObject nextIslandArrow;
	

	public void EnterShop()
	{
		SceneLoader.ChangeScenes("WorldShop");
	}

	public void StillOnNode()
	{
		nextIslandArrow.SetActive(false);
		enterShopObject.SetActive(false);
		stillOnNodeButton.SetActive(true);
		buttonHeader.text = "Market";
		buttonAction.text = "Enter";
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(Enable);
	}

	public void Enable()
	{
		nextIslandArrow.SetActive(false);
		enterShopObject.SetActive(true);
		stillOnNodeButton.SetActive(false);
	}

	public void Disable()
	{
		nextIslandArrow.SetActive(true);
		enterShopObject.SetActive(false);
		stillOnNodeButton.SetActive(false);
	}


	
}
