using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class OfferConfirmation : MonoBehaviour
{
	public static OfferConfirmation instance;

	public GameObject confirmationObject;
	public Text offerName;
	public CardDisplay cardDisplay;
	public Button purchaseButton;
	public Button closeButton;
	public Text quantityText;
	public Text purchased;
	public Text priceText;
	public Text notEnoughGoldWarning;
    public Text freeText;

	public GameObject goldIcon;
	public GameObject energyIcon;
	public GameObject gemIcon;
    public GameObject scrollsIcon;
    public GameObject goldPriceIcon;
	public GameObject gemPriceIcon;
	public GameObject kongPriceIcon;

	bool animatingRandomOffer;
	bool animatingCurrencyOffer;
	bool purchaseAnimationComplete;
	bool purchaseDataReturned;
	float increasingAmountOwned;
	float decreasingQuantity;
    int target;
    int quantity;
	Color priceTextColor = new Color();

	RandomOffer currentRandomOffer;
	CurrencyOffer currentCurrencyOffer;

	private void Awake()
	{
		priceTextColor = priceText.color;
        instance = this;
	}

	private void ResetOffer()
	{
		confirmationObject.SetActive(true);
		purchaseButton.gameObject.SetActive(true);
        priceText.gameObject.SetActive(true);
        purchased.enabled = false;
		animatingRandomOffer = false;
		animatingCurrencyOffer = false;
		purchaseAnimationComplete = false;
		purchaseDataReturned = false;
		if(goldPriceIcon != null)
		{
			goldPriceIcon.SetActive(false);
			gemPriceIcon.SetActive(false);
			kongPriceIcon.SetActive(false);
			gemIcon.SetActive(false);
			goldIcon.SetActive(false);
			energyIcon.SetActive(false);
            if(freeText != null)
            {
                freeText.gameObject.SetActive(false);
            }
            if(scrollsIcon != null)
            {
                scrollsIcon.SetActive(false);
            }
            cardDisplay.upgradeIcon.enabled = false;
			cardDisplay.typeBackground.gameObject.SetActive(false);
		}
	}

	public void SetRandomOfferConfirmation(RandomOffer randomOffer)
	{
		ResetOffer();

		cardDisplay.typeBackground.gameObject.SetActive(true);

		if (goldPriceIcon != null)
		{
			cardDisplay.upgradeIcon.enabled = true;
			goldPriceIcon.SetActive(true);
		}

		currentRandomOffer = randomOffer;

		offerName.text = randomOffer.cardDisplay.cardData.displayName;
		quantityText.text = "x" + randomOffer.quantity;
		cardDisplay.SetCardDisplay(randomOffer.cardDisplay.cardData);
		if(randomOffer.price <= Data.instance.currency.gold)
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(PurchaseRandomOffer);
			priceText.color = priceTextColor;
		}
		else
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(NotEnoughGoldWarning);
			priceText.color = Color.red;
		}
		priceText.text = randomOffer.price.ToString();
        if(randomOffer.price == 0)
        {
            if (freeText != null)
            {
                freeText.gameObject.SetActive(true);
            }
            priceText.gameObject.SetActive(false);
            goldPriceIcon.SetActive(false);
        }
	}

	public void SetCurrencyOfferConfirmation(CurrencyOffer currencyOffer)
	{
		ResetOffer();

		cardDisplay.uIProgressBar.value = 0f;
		cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
		cardDisplay.iconFrame.sprite = Resources.Load<Sprite>("UI/DefaultIconFrame");
		cardDisplay.fillBackground.sprite = Resources.Load<Sprite>("UI/DefaultFillBackground");

		currentCurrencyOffer = currencyOffer;

		offerName.text = currencyOffer.nameText.text;
		quantityText.text = currencyOffer.quantityText.text;
		priceText.text = currencyOffer.priceText.text;
		cardDisplay.cardImage.sprite = currencyOffer.image.sprite;

		if(currentCurrencyOffer.offerType == "Gem")
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(PurchasePremiumCurrencyOffer);
			priceText.color = priceTextColor;
		}
		else if (Data.instance.currency.gems <= currencyOffer.cost)
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(NoEnoughGemsWarning);
			priceText.color = Color.red;
		}
		else
		{
			purchaseButton.onClick.RemoveAllListeners();
			purchaseButton.onClick.AddListener(PurchaseCurrencyOffer);
			priceText.color = priceTextColor;
		}
		
		if(currencyOffer.offerType == "LootBag")
		{
			cardDisplay.ownedAmountText.text = Data.instance.currency.energy.ToString();
			energyIcon.SetActive(true);
			gemPriceIcon.SetActive(true);
			currencyOffer.target = currencyOffer.quantity + Data.instance.currency.energy;
		}
		else if (currencyOffer.offerType == "Gold")
		{
			cardDisplay.ownedAmountText.text = Data.instance.currency.gold.ToString();
			goldIcon.SetActive(true);
			gemPriceIcon.SetActive(true);
			currencyOffer.target = currencyOffer.quantity + Data.instance.currency.gold;
		}
		else if (currencyOffer.offerType == "Gem")
		{
			cardDisplay.ownedAmountText.text = Data.instance.currency.gems.ToString();
			gemIcon.SetActive(true);
			kongPriceIcon.SetActive(true);
			currencyOffer.target = currencyOffer.quantity + Data.instance.currency.gems;
        }
        else if (currencyOffer.offerType == "Scroll")
        {
            cardDisplay.ownedAmountText.text = Data.instance.currency.scrolls.ToString();
            scrollsIcon.SetActive(true);
            gemPriceIcon.SetActive(true);
            currencyOffer.target = currencyOffer.quantity + Data.instance.currency.scrolls;
        }
    }

	public void PurchasePremiumCurrencyOffer()
	{
#if UNITY_WEBGL
		purchaseButton.gameObject.SetActive(false);
		closeButton.gameObject.SetActive(false);

		KongregateAPIBehaviour.instance.PurchasePremiumCurrencyOffer(currentCurrencyOffer);
#endif
	}

	public void OnPremiumPurchaseSuccess(string message)
    {
#if UNITY_WEBGL
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "purchasePremiumCurrency", FunctionParameter = new { kongId = KongregateAPIBehaviour.instance.kongregateId, itemIdentifier = currentCurrencyOffer.offerName, authTicket = KongregateAPIBehaviour.instance.authTicket }, GeneratePlayStreamEvent = true}, PremiumPurchasePlayfab, OnPurchaseFailure );
#endif

        target = currentCurrencyOffer.target;
        quantity = currentCurrencyOffer.quantity;
        increasingAmountOwned = currentCurrencyOffer.target - currentCurrencyOffer.quantity;
		decreasingQuantity = currentCurrencyOffer.quantity;
        purchased.text = "Purchased!";

        animatingCurrencyOffer = true;
    }

	public void PremiumPurchasePlayfab(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			if ((bool)result.FunctionResult)
			{
				purchaseDataReturned = true;

				Data.instance.currency.gems = currentCurrencyOffer.target;
			}
		}
	}

	public void OnPremiumPurchaseFailure(string message)
	{
		purchaseButton.gameObject.SetActive(true);
		closeButton.gameObject.SetActive(true);
	}

	public void PurchaseRandomOffer()
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest(){FunctionName = "purchaseDailyOfferv2", GeneratePlayStreamEvent = true, FunctionParameter = new {cardName = cardDisplay.cardData.itemID, world = currentRandomOffer.world}}, RandomOfferDataReturned, OnPurchaseFailure);
		purchaseButton.gameObject.SetActive(false);
		closeButton.gameObject.SetActive(false);
		increasingAmountOwned = cardDisplay.cardData.amountOwned;
		decreasingQuantity = currentRandomOffer.quantity;
        purchased.text = "Purchased!";

        Data.instance.collection.AddCards(currentRandomOffer.cardDisplay.cardData.itemID, currentRandomOffer.quantity);

		animatingRandomOffer = true;
	}

	public void RandomOfferDataReturned(ExecuteCloudScriptResult result)
	{
		if(result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardAddedObject;
			object amountOwnedObject;
			jsonResult.TryGetValue("cardAdded", out cardAddedObject);
			jsonResult.TryGetValue("amountOwned", out amountOwnedObject);

			string cardAdded = (string)cardAddedObject;
			int amountOwned = PlayFabSimpleJson.DeserializeObject<int>((string)amountOwnedObject);
			
			for(int i = 0; i < currentRandomOffer.shopData.Contents.Count; i++)
			{
				if(currentRandomOffer.shopData.Contents[i].CardName == cardDisplay.cardData.itemID)
				{
                    currentRandomOffer.shopData.Purchased[i] = true;
					break;
				}
			}
			currentRandomOffer.OfferPurchased();
            Data.instance.currency.gold -= currentRandomOffer.price;

			purchaseDataReturned = true;
            if(NavBar.instance != null)
            {
                NavBar.instance.SetShopNotification();
            }
		}
		else
		{
			Debug.LogError("Offer does not exist.");
		}
	}

	public void PurchaseCurrencyOffer()
	{
		var request = new PurchaseItemRequest { CatalogVersion = "Currency", ItemId = currentCurrencyOffer.offerName, Price = currentCurrencyOffer.cost, VirtualCurrency = "DI" };
		PlayFabClientAPI.PurchaseItem(request, CurrencyOfferDataReturned, OnPurchaseFailure);
		purchaseButton.gameObject.SetActive(false);
		closeButton.gameObject.SetActive(false);

        target = currentCurrencyOffer.target;
        quantity = currentCurrencyOffer.quantity;
		increasingAmountOwned = currentCurrencyOffer.target - currentCurrencyOffer.quantity;
		decreasingQuantity = currentCurrencyOffer.quantity;
        purchased.text = "Purchased!";

        animatingCurrencyOffer = true;
	}

	public void CurrencyOfferDataReturned(PurchaseItemResult purchaseItemResult)
	{
		purchaseDataReturned = true;

		if (currentCurrencyOffer.offerType == "LootBag")
		{
			Data.instance.currency.energy = currentCurrencyOffer.target;
			Data.instance.currency.gems -= currentCurrencyOffer.cost;
		}
		else if (currentCurrencyOffer.offerType == "Gold")
		{
			Data.instance.currency.gold = currentCurrencyOffer.target;
			Data.instance.currency.gems -= currentCurrencyOffer.cost;
		}
		else if (currentCurrencyOffer.offerType == "Gem")
		{
			Data.instance.currency.gems = currentCurrencyOffer.target;
			Data.instance.currency.gems -= currentCurrencyOffer.cost;
        }
        else if (currentCurrencyOffer.offerType == "Scroll")
        {
            Data.instance.currency.scrolls = currentCurrencyOffer.target;
            Data.instance.currency.gems -= currentCurrencyOffer.cost;
        }
    }

    public void QuestReward(int reward)
    {
        ResetOffer();

        cardDisplay.uIProgressBar.value = 0f;
        cardDisplay.cardFrame.sprite = Resources.Load<Sprite>("UI/DefaultFrame");
        cardDisplay.iconFrame.sprite = Resources.Load<Sprite>("UI/DefaultIconFrame");
        cardDisplay.fillBackground.sprite = Resources.Load<Sprite>("UI/DefaultFillBackground");

        offerName.text = "Reward";
        quantityText.text = reward.ToString();
        purchaseButton.gameObject.SetActive(false);
        closeButton.gameObject.SetActive(false);
        cardDisplay.cardImage.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");

        cardDisplay.ownedAmountText.text = Data.instance.currency.gold.ToString();
        goldIcon.SetActive(true);
        gemPriceIcon.SetActive(true);
        quantity = reward;
        target = reward + Data.instance.currency.gold;

        increasingAmountOwned = target - reward;
        decreasingQuantity = reward;

        animatingCurrencyOffer = true;
    }

    public void QuestRewardReceived()
    {
        closeButton.gameObject.SetActive(false);
        purchaseDataReturned = true;
        purchased.text = "Collected";

        Data.instance.currency.gold = target;
    }

    void RandomOfferDisplay()
	{
		if(animatingRandomOffer == false)
			return;

		float rate = currentRandomOffer.quantity / 1f;
		increasingAmountOwned = Mathf.MoveTowards (increasingAmountOwned, cardDisplay.cardData.amountOwned, Time.deltaTime * rate);
		decreasingQuantity = Mathf.MoveTowards (decreasingQuantity, -0.2f, Time.deltaTime * rate);
		if(Mathf.Floor(increasingAmountOwned) < cardDisplay.cardData.AmountNeeded)
		{
			cardDisplay.uIProgressBar.value = increasingAmountOwned / cardDisplay.cardData.AmountNeeded;
		}
		else
		{
			cardDisplay.uIProgressBar.value = 1.0f;
			cardDisplay.fillImage.sprite = Resources.Load<Sprite>("UI/UnitFillOrange");
		}
		if(Mathf.Ceil(decreasingQuantity) > 0)
		{
			quantityText.text = "x" + Mathf.Ceil(decreasingQuantity);
		}
		else
		{
			quantityText.text = "";
		}
		cardDisplay.ownedAmountText.text = Mathf.Floor(increasingAmountOwned) + " / " + cardDisplay.cardData.AmountNeeded;
		if(increasingAmountOwned == cardDisplay.cardData.amountOwned)
		{
			purchaseAnimationComplete = true;
		}

		if(purchaseAnimationComplete == true && purchaseDataReturned == true)
		{
			animatingRandomOffer = false;
			closeButton.gameObject.SetActive(true);
			purchased.enabled = true;
		}
	}

	void CurrencyOfferDisplay()
	{
		if (animatingCurrencyOffer == false)
			return;

		float rate = quantity / 1f;
		increasingAmountOwned = Mathf.MoveTowards(increasingAmountOwned, target, Time.deltaTime * rate);
		decreasingQuantity = Mathf.MoveTowards(decreasingQuantity, -0.2f, Time.deltaTime * rate);

		if (Mathf.Ceil(decreasingQuantity) > 0)
		{
			quantityText.text = "x" + Mathf.Ceil(decreasingQuantity);
		}
		else
		{
			quantityText.text = "";
		}
		cardDisplay.ownedAmountText.text = Mathf.Floor(increasingAmountOwned).ToString();

		if (increasingAmountOwned == target)
		{
			purchaseAnimationComplete = true;
		}

		if (purchaseAnimationComplete == true && purchaseDataReturned == true)
		{
			animatingCurrencyOffer = false;
			closeButton.gameObject.SetActive(true);
			purchased.enabled = true;
        }
	}

	private void Update()
	{
		RandomOfferDisplay();
		CurrencyOfferDisplay();
	}

	public void NoEnoughGemsWarning()
	{
		Warning.instance.Activate("Not Enough Gems!");
	}

	public void NotEnoughGoldWarning()
	{
		Warning.instance.Activate("Not Enough Gems!");
	}

	private void OnPurchaseFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
	
}
