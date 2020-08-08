using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ChestContentsDisplay : MonoBehaviour
{
    public static ChestContentsDisplay instance;

    public GameObject confirmationObject;

    //ALL CHESTS
    public RectTransform rect;
    public Text pageHeaderText;
    public Text chestNameText;
    public CardDisplay[] cardDisplays;
    public CardDisplay jackpotCardDisplay;
    public Text cardAmountText;
    public Text goldAmountText;
    public Text starAmountText;
    public Image chestImage;

    //CHEST SLOT
    public GameObject chestSlotObject;
    public Button unlockButton;
    public Button openNowButton;
    public Button trashButton;
    public Text priceText;
    public Text descText;
    public Text timerText;
    
    //RAID INFO
    public GameObject raidInfoObject;
    public Text difficultyText;
    public Text raidPointsText;
    
    //PREMIUM CHEST
    public Button purchaseButton;

    //BACKGROUND HEIGHTS
    public float confirmationHeight;
    public float chestSlotHeight;
    public float raidHeight;

    ChestData chestData;
    ChestSlotDisplay chestSlotDisplay;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (chestSlotDisplay != null && chestSlotDisplay.unlocking)
        {
            TimeSpan t = chestSlotDisplay.chestSlot.dateTime - DateTime.UtcNow;
            string timer = "";
            if (t.Hours != 0)
            {
                timer += t.Hours + "h ";
            }
            if (t.Minutes != 0)
            {
                timer += t.Minutes + "m ";
            }
            if (t.Seconds != 0)
            {
                timer += t.Seconds + "s ";
            }
            timerText.text = timer;

			priceText.text = (Mathf.CeilToInt((float)(chestSlotDisplay.chestSlot.dateTime - DateTime.UtcNow).TotalMinutes / 20)).ToString();

			if ((chestSlotDisplay.chestSlot.dateTime - DateTime.UtcNow).TotalSeconds < 0)
            {
                confirmationObject.SetActive(false);
            }
        }
    }

    public void SetDisplay (ChestData chestData)
    {
        this.chestData = chestData;

        confirmationObject.SetActive(true);
        chestNameText.text = chestData.displayName;

        for (int i = 0; i < chestData.pool.Count; i++)
        {
            cardDisplays[i].SetCardDisplay(chestData.pool[i]);
        }
        jackpotCardDisplay.SetCardDisplay(chestData.jackpotPool[0]);
        cardAmountText.text = "Cards x" + chestData.amount;
        goldAmountText.text = "x" + chestData.gold;

        if (chestData.starChance > 99)
        {
            if (chestData.starMin == chestData.starMax)
            {
                starAmountText.text = chestData.starMin + " in chest";
            }
            else
            {
                starAmountText.text = chestData.starMin + " to " + chestData.starMax + Environment.NewLine + "in chest";
            }
        }
        else
        {
            starAmountText.text = "1 in " + Mathf.RoundToInt(100 / chestData.starChance) + Environment.NewLine + "chance";
        }
    }

    public void Confirmation(ChestData chestData)
    {
        SetDisplay(chestData);

        pageHeaderText.text = "Purchase Chest";
        rect.sizeDelta = new Vector2(rect.rect.width, confirmationHeight);
        chestSlotObject.SetActive(false);
        raidInfoObject.SetActive(false);
        purchaseButton.gameObject.SetActive(true);
        chestImage.sprite = Resources.Load<Sprite>("Chests/Default");
    }

    public void Raid(ChestData chestData, int difficulty)
    {
        SetDisplay(chestData);

        pageHeaderText.text = "Raid Reward";
        rect.sizeDelta = new Vector2(rect.rect.width, raidHeight);
        chestSlotObject.SetActive(false);
        raidInfoObject.SetActive(true);
        purchaseButton.gameObject.SetActive(false);
        chestImage.sprite = Resources.Load<Sprite>("Chests/Raid");

        difficultyText.text = "Level " + difficulty;
        raidPointsText.text = "x" + difficulty;
    }

    public void ChestSlot(ChestData chestData, ChestSlotDisplay c)
    {
        SetDisplay(chestData);

        pageHeaderText.text = "Raid Reward";
        rect.sizeDelta = new Vector2(rect.rect.width, chestSlotHeight);
        chestSlotObject.SetActive(true);
        raidInfoObject.SetActive(true);
        purchaseButton.gameObject.SetActive(false);
        chestImage.sprite = Resources.Load<Sprite>("Chests/Raid");

        difficultyText.text = "Level " + c.chestSlot.Level;
        raidPointsText.text = "x" + c.chestSlot.Level;

        chestSlotDisplay = c;

        if (c.state == ChestSlotState.Unlocking)
        {
            descText.text = "Time Left to Unlock";
            openNowButton.gameObject.SetActive(true);
            priceText.text = (Mathf.CeilToInt((float)(chestSlotDisplay.chestSlot.dateTime - DateTime.UtcNow).TotalMinutes / 20)).ToString();
            unlockButton.gameObject.SetActive(false);
            trashButton.gameObject.SetActive(true);
        }
        else if(c.state == ChestSlotState.ReadytoUnlock)
        {
            descText.text = "Time to Unlock";
            timerText.text = c.chestSlot.UnlockHours + " Hours";
            openNowButton.gameObject.SetActive(false);
            unlockButton.gameObject.SetActive(true);
            trashButton.gameObject.SetActive(true);
        }
    }

    public void StartUnlock()
    {
        chestSlotDisplay.StartUnlock();
    }

    public void OpenChest()
    {
        if(((chestSlotDisplay.chestSlot.dateTime - DateTime.UtcNow).TotalMinutes/20) < Data.instance.currency.gems)
        {
            chestSlotDisplay.OpenChestSlot();
            confirmationObject.gameObject.SetActive(false);
        }
		else
		{
			Warning.instance.Activate("Not Enough Gems");
		}
    }

    public void TrashChest()
    {
        chestSlotDisplay.TrashChest();
    }

    //PURCHASE ISLAND CHEST
    public void PurchaseChest()
    {
        if (Data.instance.chests.dict[MainShop.instance.islandChests[MainShop.instance.chestIndex]].price <= Data.instance.currency.gems)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "purchaseChest", FunctionParameter = new { chestName = MainShop.instance.islandChests[MainShop.instance.chestIndex] } }, PurchaseDataReturned, OnPurchaseFailure);
            ChestLootDisplay.instance.ChestOpening();
            Data.instance.currency.gems -= Data.instance.chests.dict[MainShop.instance.islandChests[MainShop.instance.chestIndex]].price;
            confirmationObject.gameObject.SetActive(false);
        }
        else
        {
            Warning.instance.Activate("Need more gems to purchase this chest");
        }
    }

    public void PurchaseDataReturned(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object cardsObject;
            object amountsObject;
            object goldObject;
            object gemsObject;
            object starsObject;
            jsonResult.TryGetValue("cards", out cardsObject);
            jsonResult.TryGetValue("amounts", out amountsObject);
            jsonResult.TryGetValue("gold", out goldObject);
            jsonResult.TryGetValue("gems", out gemsObject);
            jsonResult.TryGetValue("stars", out starsObject);

            string[] cardNames = PlayFabSimpleJson.DeserializeObject<string[]>((string)cardsObject);
            int[] amounts = PlayFabSimpleJson.DeserializeObject<int[]>((string)amountsObject);
            int gold = PlayFabSimpleJson.DeserializeObject<int>((string)goldObject);
            int gems = PlayFabSimpleJson.DeserializeObject<int>((string)gemsObject);
            int stars = PlayFabSimpleJson.DeserializeObject<int>((string)starsObject);
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
            Data.instance.currency.gems += gems;
            Data.instance.currency.stars += stars;

            ChestLootDisplay.instance.SetChestLootDisplay(cards, amounts, newCard, gold, gems, stars);
        }
        else
        {
            Debug.LogError("Chest does not exist.");
        }
    }

    private void OnPurchaseFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
