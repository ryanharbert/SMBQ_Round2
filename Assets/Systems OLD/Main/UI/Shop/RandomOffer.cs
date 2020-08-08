using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RandomOffer : MonoBehaviour
{
	public CardDisplay cardDisplay;
	public int price;
	public int quantity;
	public bool purchased;
    public GameObject progressBar;
    public GameObject newItem;
	public GameObject priceObject;
	public GameObject purchasedObject;
    public Text nameText;
	public Text priceText;
	public Text quantityText;
    public ShopData shopData;
    public bool world;

	public void SetRandomOffer(int i, bool world)
	{
        if (world)
        {
            shopData = Data.instance.shop.shops[Data.instance.shop.currentShop];
            this.world = true;
        }
        else
        {
            shopData = Data.instance.shop.shops["Main"];
            this.world = false;
        }
        
		cardDisplay.SetCardDisplay(shopData.Contents[i].CardName);
        nameText.text = Data.instance.collection.allCards[shopData.Contents[i].CardName].displayName;
        quantity = shopData.Contents[i].Quantity;
		quantityText.text = "x" + quantity;

        purchased = shopData.Purchased[i];
		if(purchased == false)
		{
			priceObject.SetActive(true);
			purchasedObject.SetActive(false);
			price = (quantity * 10);
            if (cardDisplay.cardData.type == CardType.Hero || cardDisplay.cardData.type == CardType.Stronghold)
            {
                price = price * 5;
            }
            if(priceText != null)
            {
                priceText.text = price.ToString();
            }
		}
		else
		{
			priceObject.SetActive(false);
			purchasedObject.SetActive(true);
		}

        if(Data.instance.collection.inventory.ContainsKey(shopData.Contents[i].CardName))
        {
            newItem.SetActive(false);
            progressBar.SetActive(true);
        }
        else
        {
            newItem.SetActive(true);
            progressBar.SetActive(false);
        }
	}

	public void OfferPurchased()
    {
        newItem.SetActive(false);
        progressBar.SetActive(true);
        cardDisplay.SetCardDisplay(cardDisplay.cardData);
		priceObject.SetActive(false);
		purchasedObject.SetActive(true);
		purchased = true;
	}

	public void OpenPurchaseConfirmation ()
	{
		if(purchased == false)
		{
			OfferConfirmation.instance.SetRandomOfferConfirmation(this);
		}
	}
}
