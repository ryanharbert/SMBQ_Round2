using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ChestConfirmation : MonoBehaviour
{
	public GameObject confirmationObject;
	public Text chestName;
	public CardDisplay[] cardDisplays;
	public CardDisplay jackpotCardDisplay;
	public Text cardAmountText;
	public Text goldAmountText;
    public Text starAmountText;
    public Button purchaseButton;
	public Text priceText;
	public Button closeButton;
	public Text notEnoughGemsWarning;

	public ChestLootDisplay chestLootDisplay;

	ChestData chestData;
	
	public void SetChestConfirmation(ChestData chestData)
	{
		this.chestData = chestData;
	
		confirmationObject.SetActive(true);

		if(chestName != null)
		{
			chestName.text = chestData.displayName;
		}
	
		for(int i = 0; i < chestData.pool.Count; i++)
		{
			cardDisplays[i].SetCardDisplay(chestData.pool[i]);
		}
		jackpotCardDisplay.SetCardDisplay(chestData.jackpotPool[0]);
		cardAmountText.text = "Cards x" + chestData.amount;
		goldAmountText.text = "x" + chestData.gold;
		if(chestData.price <= Data.instance.currency.gems)
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(PurchaseChest);
			priceText.color = Color.white;
		}
		else
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(NotEnoughGemsWarningOn);
			priceText.color = Color.red;
        }

        if (chestData.starChance > 99)
        {
            if (chestData.starMin == chestData.starMax)
            {
                starAmountText.text = chestData.starMin + " in chest";
            }
            else
            {
                starAmountText.text = chestData.starMin + " to " + chestData.starMax + System.Environment.NewLine + "in chest";
            }
        }
        else
        {
            starAmountText.text = "1 in " + Mathf.RoundToInt(100 / chestData.starChance) + System.Environment.NewLine + "chance";
        }

    }

	public void PurchaseChest()
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest(){FunctionName = "purchaseWorldShopChest", GeneratePlayStreamEvent = true}, PurchaseDataReturned, OnPurchaseFailure);
		confirmationObject.gameObject.SetActive(false);
		chestLootDisplay.ChestOpening();
	}

	public void PurchaseDataReturned(ExecuteCloudScriptResult result)
	{
		if(result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardsObject;
			object amountsObject;
            object gemsObject;
            object starsObject;
            jsonResult.TryGetValue("cards", out cardsObject);
			jsonResult.TryGetValue("amounts", out amountsObject);
            jsonResult.TryGetValue("gems", out gemsObject);
            jsonResult.TryGetValue("stars", out starsObject);

            string[] cardNames = PlayFabSimpleJson.DeserializeObject<string[]>((string)cardsObject);
			int[] amounts = PlayFabSimpleJson.DeserializeObject<int[]>((string)amountsObject);
            int gems = PlayFabSimpleJson.DeserializeObject<int>((string)gemsObject);
            int stars = PlayFabSimpleJson.DeserializeObject<int>((string)starsObject);
            bool[] newCard = new bool[cardNames.Length];

			List<CardData> cards = new List<CardData>();
			for(int i = 0; i < cardNames.Length; i++)
			{
                if(Data.instance.collection.inventory.ContainsKey(cardNames[i]))
                {
                    newCard[i] = false;
                }
                else
                {
                    newCard[i] = true;
                }

                Data.instance.collection.AddCards(cardNames[i], amounts[i]);

				CardData cardData;
				if(Data.instance.collection.inventory.TryGetValue(cardNames[i], out cardData))
				{
					cards.Add(cardData);
				}
			}

            Data.instance.currency.gold += chestData.gold;
            Data.instance.currency.gems -= chestData.price;
            Data.instance.currency.gems += gems;
            Data.instance.currency.stars += stars;

            chestLootDisplay.SetChestLootDisplay(cards, amounts, newCard, chestData.gold, gems, stars);
		}
		else
		{
			Debug.LogError("Chest does not exist.");
		}
	}

	public void NotEnoughGemsWarningOn()
	{
		notEnoughGemsWarning.enabled = true;
		Invoke("notEnoughDiamondsWarning", 1.5f);
	}

	void NotEnoughGemsWarningOff()
	{
		notEnoughGemsWarning.enabled = false;
	}

	private void OnPurchaseFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}
