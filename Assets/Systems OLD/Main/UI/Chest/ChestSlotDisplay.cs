using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class ChestSlotDisplay : MonoBehaviour {

    public int slotIndex;

    public Button button;

    public Image background;
    public Image chestImage;

    public Text backgroundText;
    public Text chestNameText;
    public Text timerText;
    public Text actionText;
    public Text costText;
    public Text unlockHoursText;
    public Text difficultyText;

    public Color inactiveColor;
    public Color unlockingColor;
    public Color unlockedColor;
    public Color unlockingTextColor;
    public Color unlockedTextColor;

    [HideInInspector] public ChestSlotState state;
    [HideInInspector] public bool unlocking = false;
    [HideInInspector] public ChestSlotData chestSlot;

    private void Update()
    {
        if(unlocking)
        {
            TimeSpan t = chestSlot.dateTime - DateTime.UtcNow;
            timerText.text = TimeSpanDisplay.Format(t);

			costText.text = (Mathf.CeilToInt((float)(chestSlot.dateTime - DateTime.UtcNow).TotalMinutes / 20)).ToString();

			if ((chestSlot.dateTime - DateTime.UtcNow).TotalSeconds < 0)
            {
                Set(chestSlot);
            }
        }
    }

    public void Set(ChestSlotData c)
    {
        chestSlot = c;
        if(c.Name == "")
        {
            unlocking = false;
            state = ChestSlotState.Empty;
            button.interactable = false;
            background.color = inactiveColor;
            chestImage.gameObject.SetActive(false);
            backgroundText.gameObject.SetActive(true);
            if (chestNameText != null)
            {
                chestNameText.gameObject.SetActive(false);
            }
            timerText.gameObject.SetActive(false);
            actionText.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            unlockHoursText.gameObject.SetActive(false);
            difficultyText.gameObject.SetActive(false);
        }
        else if(c.TimeStamp == 0)
        {
            unlocking = false;
            button.interactable = true;
            background.color = inactiveColor;
            chestImage.gameObject.SetActive(true);
            //chestImage.sprite = Resources.Load<Sprite>("Chests/" + c.Name);

			//chestImage.sprite = Resources.Load<Sprite>("Chests/Raid");

            state = ChestSlotState.ReadytoUnlock;

            backgroundText.gameObject.SetActive(false);
            chestNameText.gameObject.SetActive(true);
            chestNameText.text = c.Name;
            timerText.gameObject.SetActive(false);
            actionText.gameObject.SetActive(false);
            costText.gameObject.SetActive(false);
            unlockHoursText.gameObject.SetActive(true);
            unlockHoursText.text = c.UnlockHours + "h";
            difficultyText.gameObject.SetActive(true);
            difficultyText.text = "Level " + c.Level;
        }
        else if((chestSlot.dateTime - DateTime.UtcNow).TotalSeconds > 0)
        {
            unlocking = true;
            button.interactable = true;
            state = ChestSlotState.Unlocking;
            background.color = unlockingColor;
            chestImage.gameObject.SetActive(true);
			//chestImage.sprite = Resources.Load<Sprite>("Chests/" + c.Name);

			//chestImage.sprite = Resources.Load<Sprite>("Chests/Raid");

			backgroundText.gameObject.SetActive(false);
            if (chestNameText != null)
            {
                chestNameText.gameObject.SetActive(true);
				chestNameText.text = c.Name;
			}
            timerText.gameObject.SetActive(true);
            actionText.gameObject.SetActive(true);
            actionText.color = unlockingTextColor;
            costText.gameObject.SetActive(true);
            costText.text = (Mathf.CeilToInt((float)(chestSlot.dateTime - DateTime.UtcNow).TotalMinutes/20)).ToString();
            unlockHoursText.gameObject.SetActive(false);
            difficultyText.gameObject.SetActive(true);
            difficultyText.text = "Level " + c.Level;
        }
        else
        {
            unlocking = false;
            button.interactable = true;
            state = ChestSlotState.Open;
            background.color = unlockedColor;
            chestImage.gameObject.SetActive(true);
			//chestImage.sprite = Resources.Load<Sprite>("Chests/" + c.Name);
			
			//chestImage.sprite = Resources.Load<Sprite>("Chests/Raid");

			backgroundText.gameObject.SetActive(false);
            if (chestNameText != null)
            {
                chestNameText.gameObject.SetActive(true);
				chestNameText.text = c.Name;
            }
            timerText.gameObject.SetActive(false);
            actionText.gameObject.SetActive(true);
            actionText.color = unlockedTextColor;
            costText.gameObject.SetActive(false);
            unlockHoursText.gameObject.SetActive(false);
            difficultyText.gameObject.SetActive(true);
            difficultyText.text = "Level " + c.Level;
        }
    }

    public void SlotClicked()
    {
        if (state == ChestSlotState.Unlocking || state == ChestSlotState.ReadytoUnlock || state == ChestSlotState.Waiting)
        {
            ChestContentsDisplay.instance.ChestSlot(Data.instance.raids.GetRaidChest(chestSlot.Name, chestSlot.Level), this);
        }
        else if (state == ChestSlotState.Open)
        {
            OpenChestSlot();
        }
    }

    public void OpenChestSlot()
    {
        ChestLootDisplay.instance.ChestOpening();
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "openChestSlotv3", GeneratePlayStreamEvent = true, FunctionParameter = new { index = slotIndex } }, OpenChestSlotSuccess, StartUnlockFailure);
    }

    private void OpenChestSlotSuccess(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object chestResultObject;
			object gemCostObject;
            object pointIncreaseObject;
            object rankObject;
            jsonResult.TryGetValue("chestData", out chestResultObject);
			jsonResult.TryGetValue("gemCost", out gemCostObject);
            jsonResult.TryGetValue("pointIncrease", out pointIncreaseObject);
            jsonResult.TryGetValue("rank", out rankObject);

            object chestResult = PlayFabSimpleJson.DeserializeObject<object>((string)chestResultObject);
			int gemCost = Convert.ToInt32(gemCostObject);
            int pointIncrease = Convert.ToInt32(pointIncreaseObject);
            int rank = Convert.ToInt32(rankObject);

            if (gemCost != 0)
			{
				Data.instance.currency.gems += gemCost;
			}

			JsonObject chestResultJson = (JsonObject)chestResult;
			object cardsObject;
            object amountsObject;
            object goldObject;
            object gemsObject;
            object starsObject;
            chestResultJson.TryGetValue("cards", out cardsObject);
			chestResultJson.TryGetValue("amounts", out amountsObject);
			chestResultJson.TryGetValue("gold", out goldObject);
            chestResultJson.TryGetValue("gems", out gemsObject);
            chestResultJson.TryGetValue("stars", out starsObject);

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
            Data.instance.currency.raidPoints += pointIncrease;
            if(pointIncrease > 0)
            {
                Raid.instance.EventDisplay();
            }

            Data.instance.raids.chestSlots[slotIndex].Name = "";
            Data.instance.raids.chestSlots[slotIndex].UnlockHours = 0;
            Data.instance.raids.chestSlots[slotIndex].TimeStamp = 0;
            Data.instance.raids.SetChestSlot(Data.instance.raids.chestSlots[slotIndex]);
            Raid.instance.SetChestSlots();
            NavBar.instance.SetRaidNotification();
            

            ChestLootDisplay.instance.SetChestLootDisplay(cards, amounts, newCard, gold, gems, stars, pointIncrease, rank);
        }
        else
        {
            Debug.LogError("Chest does not exist.");
        }
    }

    public void StartUnlock()
    {
        ChestContentsDisplay.instance.unlockButton.gameObject.SetActive(false);
        ChestContentsDisplay.instance.trashButton.gameObject.SetActive(false);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "startChestUnlockv3", FunctionParameter = new { index = slotIndex } }, StartUnlockSuccess, StartUnlockFailure);
    }

    private void StartUnlockSuccess(ExecuteCloudScriptResult result)
    {
        ChestSlotData c = PlayFabSimpleJson.DeserializeObject<ChestSlotData>((string)result.FunctionResult);

        Data.instance.raids.chestSlots[slotIndex] = c;
        Data.instance.raids.SetChestSlot(Data.instance.raids.chestSlots[slotIndex]);
        Raid.instance.SetChestSlots();
        NavBar.instance.SetRaidNotification();

        ChestContentsDisplay.instance.confirmationObject.SetActive(false);
    }

    public void TrashChest()
    {
        ChestContentsDisplay.instance.openNowButton.gameObject.SetActive(false);
        ChestContentsDisplay.instance.unlockButton.gameObject.SetActive(false);
        ChestContentsDisplay.instance.trashButton.gameObject.SetActive(false);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "trashChest", FunctionParameter = new { index = slotIndex } }, TrashChestSuccess, StartUnlockFailure);
    }

    private void TrashChestSuccess(ExecuteCloudScriptResult result)
    {
        Data.instance.raids.chestSlots[slotIndex].Name = "";
        Data.instance.raids.chestSlots[slotIndex].UnlockHours = 0;
        Data.instance.raids.chestSlots[slotIndex].TimeStamp = 0;
        Data.instance.raids.SetChestSlot(Data.instance.raids.chestSlots[slotIndex]);
        Raid.instance.SetChestSlots();
        NavBar.instance.SetRaidNotification();

        ChestContentsDisplay.instance.confirmationObject.SetActive(false);
    }

    private void StartUnlockFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
