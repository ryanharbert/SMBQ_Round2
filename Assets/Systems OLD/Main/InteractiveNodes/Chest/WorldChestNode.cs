using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class WorldChestNode : InteractiveNode
{
    public Animator anim;

    public override bool Setup()
    {
        if(Data.instance.world.worldChests.treasures.ContainsKey(name) && Data.instance.world.worldChests.treasures[name] != 0)
        {
            return true;
        }
        else
        {
            gameObject.SetActive(false);
            return false;
        }
    }

    public override void EnterRange()
    {
        WorldManager.instance.ToggleWorld(false);
        anim.SetBool("Open", true);
        Invoke("GetWorldChestLoot", 1.5f);
    }

    void GetWorldChestLoot()
    {
        WorldManager.instance.uiManager.chestLootDisplay.ChestOpening();
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "V2GetWorldChest", GeneratePlayStreamEvent = true, FunctionParameter = new { chestNode = name } }, ChestDataReturned, WorldManager.instance.ServerFailure);
    }

    public void ChestDataReturned(ExecuteCloudScriptResult result)
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

            WorldManager.instance.uiManager.chestLootDisplay.SetChestLootDisplay(cards, amounts, newCard, gold, gems, stars);

            //UPDATE WORLD AND DATA
            gameObject.SetActive(false);
            WorldManager.instance.interactiveNodes.Remove(this);
            Data.instance.world.worldChests.treasures[name] = 0;

            WorldManager.instance.ToggleWorld(true);
        }
        else
        {
            Debug.LogError("Chest does not exist.");
        }
    }
}
