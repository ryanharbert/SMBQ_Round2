using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using System;

public class WorldBattle
{

	void GetLoot(BattleType type)
	{
		if (battleType == BattleType.World)
		{
            if(Data.instance.battle.enemyName != "Tutorial")
            {
                EnemyData tempE;
                if (Data.instance.battle.enemyName == "Mimic" && !Data.instance.world.Enemies.TryGetValue(Data.instance.world.CurrentPlayerNode, out tempE))
			    {
				    Data.instance.world.worldChests.treasures[Data.instance.world.CurrentPlayerNode] = false;
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "collectWorldLoot", FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = true } }, LootReceived, OnLootFailure);
                }
                else
                {
                    if (Data.instance.world.Enemies.ContainsKey(Data.instance.world.CurrentPlayerNode))
                    {
                        Data.instance.world.Enemies.Remove(Data.instance.world.CurrentPlayerNode);
                    }
                    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getLootTable", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName } }, GetPossibleLoot, OnLootFailure);
                }
            }
            else
            {
                Data.instance.tutorial.steps["BattleComplete"] = true;
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "tutorialBattleWinv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = "Tutorial", FrameRate = FrameRate.instance.avgFrameRate } }, LootReceived, OnLootFailure);
            }
        }
		else if (battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "fillChestSlotv4", GeneratePlayStreamEvent = true, FunctionParameter = new { level = Data.instance.raidBattle.level, raidName = Data.instance.raidBattle.enemyName } }, ChestSlotFilledSuccess, ChestSlotFilledFailure);
		}
    }

    public void CollectLootWorld(bool collect)
	{
		PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
		if (collect)
        {
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "worldBattleLootv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = collect } }, LootBagReceived, OnLootFailure);
        }
        else
        {
			PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "worldBattleLootv2", GeneratePlayStreamEvent = true, FunctionParameter = new { enemy = Data.instance.battle.enemyName, collect = collect } }, IgnoreLeaveBattle, OnLootFailure);
        }
    }
    

	private void GetPossibleLoot(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            string[] cards = PlayFabSimpleJson.DeserializeObject<string[]>(result.FunctionResult.ToString());

            battleEnd.WorldBattle(cards);
        }
        else
        {
            Debug.LogError("Enemy drop pool does NOT exist.");
        }
	}

	private void LootReceived(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardObject;
			object amountObject;
			object goldObject;
			object expObject;
			object levelObject;
			jsonResult.TryGetValue("cardAdded", out cardObject);
			jsonResult.TryGetValue("amountOwned", out amountObject);
			jsonResult.TryGetValue("goldAdded", out goldObject);
			jsonResult.TryGetValue("expAdded", out expObject);
			jsonResult.TryGetValue("playerLevel", out levelObject);

			string cardAdded = (string)cardObject;

			int amountAdded = 0;
			int amountOwned = Convert.ToInt32(amountObject);
			int goldAdded = Convert.ToInt32(goldObject);
			int expAdded = Convert.ToInt32(expObject);
			int playerLevel = Convert.ToInt32(levelObject);

			bool newItem = false;
			CardData card;
			if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned - 1 - card.amountOwned;
			}
			else if (Data.instance.collection.allCards.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned;
				newItem = true;
			}

			Data.instance.collection.AddCards(cardAdded, amountAdded);

			Data.instance.currency.gold += goldAdded;
			Data.instance.currency.IncreaseExp(expAdded, playerLevel);

			battleEnd.LootReceived(card, amountAdded, goldAdded, expAdded, newItem);
		}
		else
		{
			Debug.LogError("Enemy drop pool does NOT exist.");
		}
	}

	private void LootBagReceived(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			JsonObject jsonResult = (JsonObject)result.FunctionResult;
			object cardObject;
			object amountObject;
			object goldObject;
			object expObject;
			object levelObject;
			jsonResult.TryGetValue("cardAdded", out cardObject);
			jsonResult.TryGetValue("amountOwned", out amountObject);
			jsonResult.TryGetValue("goldAdded", out goldObject);
			jsonResult.TryGetValue("expAdded", out expObject);
			jsonResult.TryGetValue("playerLevel", out levelObject);

			string cardAdded = (string)cardObject;

			int amountAdded = 0;
			int amountOwned = Convert.ToInt32(amountObject);
			int goldAdded = Convert.ToInt32(goldObject);
			int expAdded = Convert.ToInt32(expObject);
			int playerLevel = Convert.ToInt32(levelObject);

            bool newItem = false;
			CardData card;
			if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
			{
				amountAdded = amountOwned - 1 - card.amountOwned;
			}
			else if(Data.instance.collection.allCards.TryGetValue(cardAdded, out card))
			{
                amountAdded = amountOwned;
                newItem = true;
			}

            Data.instance.collection.AddCards(cardAdded, amountAdded);

			Data.instance.currency.gold += goldAdded;
			Data.instance.currency.IncreaseExp(expAdded, playerLevel);

			battleEnd.LootReceived(card, amountAdded, goldAdded, expAdded, newItem);
		}
		else
		{
			Debug.LogError("Enemy drop pool does NOT exist.");
		}
	}

	private void OnLootFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}
