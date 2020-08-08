using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class CurrencyOffer : MonoBehaviour
{
	public string offerName;
	public Text nameText;
	public Text priceText;
	public Text quantityText;
	public Image image;
	public string offerType;
	public int cost;
	public int quantity;
	public int target;

	public void SetCurrencyOffer(CatalogItem catalogItem)
	{
		nameText.text = catalogItem.DisplayName;
		//image.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");

		if (offerName.Contains("LB"))
		{
			cost = (int)catalogItem.VirtualCurrencyPrices["DI"];
			priceText.text = cost.ToString();
			quantity = (int)catalogItem.Bundle.BundledVirtualCurrencies["LB"];
			quantityText.text = "x" + quantity;
			offerType = "LootBag";
		}
		else if(offerName.Contains("GO"))
		{
			cost = (int)catalogItem.VirtualCurrencyPrices["DI"];
			priceText.text = cost.ToString();
			quantity = (int)catalogItem.Bundle.BundledVirtualCurrencies["GO"];
			quantityText.text = "x" + quantity;
			offerType = "Gold";
		}
		else if (offerName.Contains("gem"))
		{
#if UNITY_WEBGL
			cost = Data.instance.kongOffers[catalogItem.ItemId];
#else
			cost = 0;
#endif
			priceText.text = cost.ToString();
			quantity = (int)catalogItem.Bundle.BundledVirtualCurrencies["DI"];
			quantityText.text = "x" + quantity;
			offerType = "Gem";
        }
        else if (offerName.Contains("SC"))
        {
            cost = (int)catalogItem.VirtualCurrencyPrices["DI"];
            priceText.text = cost.ToString();
            quantity = (int)catalogItem.Bundle.BundledVirtualCurrencies["SC"];
            quantityText.text = "x" + quantity;
            offerType = "Scroll";
        }
    }

	public void OfferPurchaseConfirmation()
	{
		OfferConfirmation.instance.SetCurrencyOfferConfirmation(this);
	}
}
