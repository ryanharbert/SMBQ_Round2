using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class MainShop : MonoBehaviour
{
	public static MainShop instance;

    public NavBarToggle daily;
    public NavBarToggle chests;
    public NavBarToggle currencies;

    public GameObject loadingObject;
    public GameObject contentObject;
    public GameObject chestObject;
    public GameObject[] currenciesObjects;
    public GameObject dailyObject;

    public SpecialDeal deal;
    public GameObject dealObject;
    public GameObject newDealTimerObject;
    public Text newDealTimerText;

    public RectTransform contentRect;
    public Text shopResetTimer;


    public RandomOffer[] randomOffers;
	public CurrencyOffer[] currencyOffers;
    
	public Dictionary<string, CatalogItem> catalogItemDict = new Dictionary<string, CatalogItem>();

	//CHESTS
	public Button purchaseButton;
	public ChestDisplay chestDisplay;
	public GameObject chestDisplayObject;
	public GameObject chestLockedObject;
	public Text chestLockedText;
    public Text chestIndexText;
    public List<string> islandChests = new List<string>();
	public List<string> islandReq = new List<string>();
	public List<bool> chestsUnlocked = new List<bool>();
	public int chestIndex = 0;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        SetShop();
    }

    void SetShop()
    {
        catalogItemDict = new Dictionary<string, CatalogItem>();
        List<CatalogItem> catalogItemList = Data.instance.shop.currencyOffers.Catalog;
        for (int i = 0; i < catalogItemList.Count; i++)
        {
            catalogItemDict.Add(catalogItemList[i].ItemId, catalogItemList[i]);
        }
        for (int i = 0; i < currencyOffers.Length; i++)
        {
            CatalogItem catalogItem;
            if (catalogItemDict.TryGetValue(currencyOffers[i].offerName, out catalogItem))
            {
                currencyOffers[i].SetCurrencyOffer(catalogItem);
            }
        }

        if (chestDisplay != null)
        {
            islandChests = new List<string>();
            islandReq = new List<string>();
            chestsUnlocked = new List<bool>();
            foreach (KeyValuePair<string, IslandData> k in Data.instance.world.islands)
            {
                if (Data.instance.world.VisitedIslands.Contains(k.Key))
                {
                    chestIndex = chestsUnlocked.Count;
                }
                foreach (string s in k.Value.Chests)
                {
                    islandChests.Add(s);
                    islandReq.Add(k.Key);
                    if (Data.instance.world.VisitedIslands.Contains(k.Key))
                    {
                        chestsUnlocked.Add(true);
                    }
                    else
                    {
                        chestsUnlocked.Add(false);
                    }
                }
            }

            SetChestDisplay();
        }

        if (deal != null)
        {
            SetSpecialDeal();
        }

        if (daily != null)
        {
            daily.toggle.isOn = false;
            chests.toggle.isOn = false;
            currencies.toggle.isOn = false;
            daily.toggle.isOn = true;
        }

        if (Data.instance.shop.shops == null || Data.instance.shop.month != DateTime.UtcNow.Month || Data.instance.shop.day != DateTime.UtcNow.Day)
        {
            Data.instance.shop.month = DateTime.UtcNow.Month;
            Data.instance.shop.day = DateTime.UtcNow.Day;
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "resetUserShop" }, ShopDataReturned, Data.instance.GetDataFailure);
            contentObject.SetActive(false);
            loadingObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < randomOffers.Length; i++)
            {
                randomOffers[i].SetRandomOffer(i, false);
                if (i == 0)
                {
                    randomOffers[i].price = 0;
                }
            }
        }
    }

    public void SetSpecialDeal()
    {
        DealData d = Data.instance.deals.GetCurrentDeal();

        if (d != null)
        {
            newDealTimerObject.SetActive(false);
            dealObject.SetActive(true);
            deal.Set(d);
        }
        else
        {
            newDealTimerObject.SetActive(true);
            dealObject.SetActive(false);
            TimeSpan t = Data.instance.deals.dealChange - DateTime.UtcNow;
            newDealTimerText.text = TimeSpanDisplay.Format(t);
        }
        //daily.toggle.group.NotifyToggleOn(daily.toggle);
    }

    public void ShopDataReturned(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            Data.instance.shop.shops = PlayFabSimpleJson.DeserializeObject<Dictionary<string, ShopData>>(result.FunctionResult.ToString());


            foreach (KeyValuePair<string, ShopData> s in Data.instance.shop.shops)
            {
                s.Value.Purchased = new List<bool>();
                for (int i = 0; i < s.Value.Contents.Count; i++)
                {
                    s.Value.Purchased.Add(false);
                }
            }
            SetShop();
            contentObject.SetActive(true);
            loadingObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Offer does not exist.");
        }
    }

    private void Update()
    {
		if(shopResetTimer != null)
        {
            if (Data.instance.shop.shops == null || Data.instance.shop.month != DateTime.UtcNow.Month || Data.instance.shop.day != DateTime.UtcNow.Day)
            {
                Data.instance.shop.month = DateTime.UtcNow.Month;
                Data.instance.shop.day = DateTime.UtcNow.Day;
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "resetUserShop", GeneratePlayStreamEvent = true }, ShopDataReturned, Data.instance.GetDataFailure);
                contentObject.SetActive(false);
                loadingObject.SetActive(true);
            }
            else
            {
                TimeSpan t = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1) - DateTime.UtcNow;
                shopResetTimer.text = "New Offers In " + TimeSpanDisplay.Format(t);
            }
        }

        if (deal == null)
            return;


        if(Data.instance.deals.currentDeal != "")
        {
            if (Data.instance.deals.dealChange > DateTime.UtcNow)
            {
                TimeSpan t = Data.instance.deals.dealChange - DateTime.UtcNow;
                deal.timeLeftText.text = "Time Left: " + TimeSpanDisplay.Format(t);
            }
            else
            {
                SetSpecialDeal();
            }
        }
        else
        {
            if (Data.instance.deals.dealChange > DateTime.UtcNow)
            {
                TimeSpan t = Data.instance.deals.dealChange - DateTime.UtcNow;
                newDealTimerText.text = TimeSpanDisplay.Format(t);
            }
            else
            {
                SetSpecialDeal();
            }
        }
    }

    public void ChangeChestLeft()
    {
        chestIndex--;
        if(chestIndex < 0)
        {
            chestIndex = islandChests.Count - 1;
        }
		SetChestDisplay();
	}

    public void ChangeChestRight()
    {
        chestIndex++;
        if (chestIndex > (islandChests.Count - 1))
        {
            chestIndex = 0;
        }
		SetChestDisplay();
    }

	void SetChestDisplay()
	{
		chestIndexText.text = (chestIndex + 1) + " / " + islandChests.Count;
		chestDisplay.Set(islandChests[chestIndex]);
		if(chestsUnlocked[chestIndex])
		{
			chestLockedObject.SetActive(false);
			chestDisplayObject.SetActive(true);
			purchaseButton.interactable = true;
		}
		else
		{
			chestLockedObject.SetActive(true);
			chestDisplayObject.SetActive(false);
			chestLockedText.text = "Must reach " + Data.instance.world.GetIslandName(islandReq[chestIndex]) + " to unlock";
			purchaseButton.interactable = false;
		}
	}

    public void ChestConfirmation()
    {
        ChestContentsDisplay.instance.Confirmation(Data.instance.chests.dict[islandChests[chestIndex]]);
	}

    public void Daily(bool on)
    {
        if(on)
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
            contentObject.SetActive(true);
            dailyObject.SetActive(true);
            foreach (GameObject g in currenciesObjects)
            {
                g.SetActive(false);
            }
            chestObject.SetActive(false);
        }
    }

    public void Chests(bool on)
    {
        if (on)
        {
            contentObject.SetActive(false);
            chestObject.SetActive(true);
        }
    }

    public void Currencies(bool on)
    {
        if (on)
        {
            contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);
            contentObject.SetActive(true);
            dailyObject.SetActive(false);
            foreach (GameObject g in currenciesObjects)
            {
                g.SetActive(true);
            }
            chestObject.SetActive(false);
        }
    }
}
