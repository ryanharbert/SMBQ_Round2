using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Chest : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public Text chestNameText;
	public CardDisplay[] cardDisplays;
	public CardDisplay jackpotCardDisplay;
	public Animator chestAnim;
	public Text priceText;
	public ChestConfirmation chestConfirmation;

	ChestData chestData;

	public void SetChest()
	{
        chestData = Data.instance.chests.dict[Data.instance.shop.currentShop];

		chestNameText.text = chestData.displayName;
		for(int i = 0; i < chestData.pool.Count; i++)
		{
			cardDisplays[i].SetCardDisplay(chestData.pool[i]);
		}
		jackpotCardDisplay.SetCardDisplay(chestData.jackpotPool[0]);
		priceText.text = chestData.price.ToString();
	}
	
	public void OnPointerDown (PointerEventData data)
	{
		chestAnim.SetBool("Open", true);
	}

	public void OnPointerUp (PointerEventData data)
	{
		chestAnim.SetBool("Open", false);
		
		chestConfirmation.SetChestConfirmation(chestData);
	}
}
