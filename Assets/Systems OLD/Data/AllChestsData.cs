using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class AllChestsData
{
    public Dictionary<string, ChestData> dict;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        string chestDataJson;

        if (playerInfo.TitleData.TryGetValue("Chests", out chestDataJson))
        {
            dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, ChestData>>(chestDataJson);
        }
    }

    public void OpenChest(JsonObject jsonResult)
    {
        object cardsObject;
        object amountsObject;
        object goldObject;
        object gemsObject;
        object starsObject;

        List<CardData> cards = new List<CardData>();
        bool[] newCard = null;
        int[] amounts = null;
        int gold = 0;
        int gems = 0;
        int stars = 0;

        if (jsonResult.TryGetValue("cards", out cardsObject) && jsonResult.TryGetValue("amounts", out amountsObject))
        {
            string[] cardNames = PlayFabSimpleJson.DeserializeObject<string[]>((string)cardsObject);
            amounts = PlayFabSimpleJson.DeserializeObject<int[]>((string)amountsObject);

            newCard = new bool[cardNames.Length];

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
        }
        if (jsonResult.TryGetValue("gold", out goldObject))
        {
            gold = PlayFabSimpleJson.DeserializeObject<int>((string)goldObject);
            Data.instance.currency.gold += gold;
        }
        if (jsonResult.TryGetValue("gems", out gemsObject))
        {
            gems = PlayFabSimpleJson.DeserializeObject<int>((string)gemsObject);
            Data.instance.currency.gems += gems;
        }
        if (jsonResult.TryGetValue("stars", out starsObject))
        {
            stars = PlayFabSimpleJson.DeserializeObject<int>((string)starsObject);
            Data.instance.currency.stars += stars;
        }


        ChestLootDisplay.instance.SetChestLootDisplay(cards, amounts, newCard, gold, gems, stars);
    }
}
