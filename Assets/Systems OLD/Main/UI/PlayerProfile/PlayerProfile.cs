using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfile : MonoBehaviour {

	public GameObject profileObject;
	public Text playerName;
	public Text playerLevel;
	public Text cardLevelMax;

	public void Set()
	{
		profileObject.SetActive(true);
		playerName.text = Data.instance.user.displayName;
		playerLevel.text = "Player Level: " + Data.instance.currency.playerLevel;
		cardLevelMax.text = "Max Card Level: " + (Data.instance.currency.playerLevel + 5);
	}
}
