using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class SpecialDeal : MonoBehaviour
{
    public Text timeLeftText;
    public Text titleText;
    public Text descText;
    
    public GridLayoutGroup grid;
    public CardDisplay[] cardsInOffer;
    public Text[] cardAmountsText;
    
    public RectTransform singleCardRect;
    public CardDisplay[] cardsInOfferUnder3;
    public Text[] cardAmountsTextUnder3;

    public Text goldAmountText;
    public Text staminaAmountText;
    public Text scrollsAmountText;

    public GameObject goldObject;
    public GameObject staminaObject;
    public GameObject scrollsObject;
    
    public Text priceText;

	public GameObject confirmObject;
	public Text confirmPriceText;
	public Text confirmDescText;

	DealData dealData;

    public void Set(DealData deal)
    {
		dealData = deal;
        titleText.text = deal.DisplayName;
        descText.text = deal.Description;
        if(deal.Cards.Length > 2)
        {
            if (deal.Cards.Length > 4)
            {
                grid.constraintCount = 3;
            }
            else
            {
                grid.constraintCount = 2;
            }
            for (int i = 0; i < cardsInOffer.Length; i++)
            {
                if (deal.Cards.Length > i)
                {
                    cardsInOffer[i].gameObject.SetActive(true);
                    cardsInOffer[i].SetCardDisplay(deal.Cards[i].Name);
                    cardAmountsText[i].text = "x" + deal.Cards[i].Amount;
                }
                else
                {
                    cardsInOffer[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < cardsInOfferUnder3.Length; i++)
            {
                cardsInOfferUnder3[i].gameObject.SetActive(false);
            }
        }
        else
        {
            if (deal.Cards.Length > 1)
            {
                singleCardRect.localScale = new Vector3(1, 1);
            }
            else
            {
                singleCardRect.localScale = new Vector3(1.2f, 1.2f);
            }
            for (int i = 0; i < cardsInOfferUnder3.Length; i++)
            {
                if (deal.Cards.Length > i)
                {
                    cardsInOfferUnder3[i].gameObject.SetActive(true);
                    cardsInOfferUnder3[i].SetCardDisplay(deal.Cards[i].Name);
                    cardAmountsTextUnder3[i].text = "x" + deal.Cards[i].Amount;
                }
                else
                {
                    cardsInOfferUnder3[i].gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < cardsInOffer.Length; i++)
            {
                cardsInOffer[i].gameObject.SetActive(false);
            }
        }
        SetCurrencyAmount(deal.Gold, goldAmountText, goldObject);
        SetCurrencyAmount(deal.Stamina, staminaAmountText, staminaObject);
        SetCurrencyAmount(deal.Scrolls, scrollsAmountText, scrollsObject);
        priceText.text = deal.Price.ToString();
		confirmPriceText.text = deal.Price.ToString();
		confirmDescText.text = "Purchase " + deal.DisplayName + " for " + deal.Price + " gems?";
	}

    void SetCurrencyAmount(int amount, Text amountText, GameObject mainObject)
    {
        if (amount > 0)
        {
            mainObject.SetActive(true);
            amountText.text = "x" + amount;
        }
        else
        {
            mainObject.SetActive(false);
        }
    }

	public void OpenConfirmation()
	{
		if(Data.instance.currency.gems >= dealData.Price)
		{
			confirmObject.SetActive(true);
		}
		else
		{
			Warning.instance.Activate("You need more gems to purchase this deal.");
		}
	}

	public void PurchaseDeal()
	{
		confirmObject.SetActive(false);
		ChestLootDisplay.instance.ChestOpening();
		Data.instance.currency.gems -= dealData.Price;
		if(Data.instance.deals.currentDeal == "NewUser")
		{
			Data.instance.deals.NewUserPurchased = true;
		}
		else
		{
			Data.instance.deals.CurrentDealName = Data.instance.deals.currentDeal;
			Data.instance.deals.CurrentDealPurchased = true;
		}
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "purchaseSpecialDealv2", FunctionParameter = new { deal = Data.instance.deals.currentDeal } }, PurchaseDataReturned, OnPurchaseFailure);
	}

	public void PurchaseDataReturned(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardsObject;
			object amountsObject;
			object goldObject;
            jsonResult.TryGetValue("cards", out cardsObject);
			jsonResult.TryGetValue("amounts", out amountsObject);
			jsonResult.TryGetValue("gold", out goldObject);

            string[] cardNames = PlayFabSimpleJson.DeserializeObject<string[]>((string)cardsObject);
			int[] amounts = PlayFabSimpleJson.DeserializeObject<int[]>((string)amountsObject);
			int gold = System.Convert.ToInt32(goldObject);
			int gems = 0;
			bool[] newCard = new bool[cardNames.Length];

			List<CardData> cards = new List<CardData>();
			for (int i = 0; i < cardNames.Length; i++)
			{
				if (Data.instance.collection.inventory.ContainsKey(cardNames[i]))
				{
					newCard[i] = false;
				}
				else
				{
					newCard[i] = true;
				}

				Data.instance.collection.AddCards(cardNames[i], amounts[i]);

				CardData cardData;
				if (Data.instance.collection.inventory.TryGetValue(cardNames[i], out cardData))
				{
					cards.Add(cardData);
				}
			}

			Data.instance.currency.gold += gold;

            ChestLootDisplay.instance.SetChestLootDisplay(cards, amounts, newCard, gold, gems, 0);
			MainShop.instance.SetSpecialDeal();
		}
		else
		{
			Debug.LogError("Deal does not exist.");
		}
	}

	private void OnPurchaseFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}
