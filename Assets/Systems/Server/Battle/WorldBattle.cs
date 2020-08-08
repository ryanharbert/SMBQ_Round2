using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using System;

namespace SMBQ.Data
{
	public class WorldBattle
	{
		private const string PossibleLootFunctionName = "getLootTable";
		private const string CollectLootFunctionName = "worldBattleLootv2";

		#region Possible Loot
		private Action possibleLootReceived;
		
		public void GetPossibleLoot(string enemyName, Action possibleLootReceived)
		{
			// REMOVE ENEMY
			// if (Data.instance.world.Enemies.ContainsKey(Data.instance.world.CurrentPlayerNode))
			// {
			// 	Data.instance.world.Enemies.Remove(Data.instance.world.CurrentPlayerNode);
			// }
			
			if (Data.Instance.offline)
			{
				possibleLootReceived.Invoke();
			}
			else
			{
				this.possibleLootReceived = possibleLootReceived;
				PlayFabClientAPI.ExecuteCloudScript(
					new ExecuteCloudScriptRequest()
					{
						FunctionName = PossibleLootFunctionName, GeneratePlayStreamEvent = true,
						FunctionParameter = new {enemy = enemyName}
					}, ReceivedPossibleLoot, GetPossibleLootFailure);
			}

		}
		
		void ReceivedPossibleLoot(ExecuteCloudScriptResult result)
		{
			if (result.FunctionResult != null)
			{
				string[] cards = PlayFabSimpleJson.DeserializeObject<string[]>(result.FunctionResult.ToString());

				possibleLootReceived.Invoke();
			}
			else
			{
				Debug.LogError("Enemy drop pool does NOT exist.");
			}
		}

		private void GetPossibleLootFailure(PlayFabError error)
		{
			Debug.LogError("GetPossibleLoot FAILURE");
			Debug.LogError("Here's some debug information:");
			Debug.LogError(error.GenerateErrorReport());
		}
		#endregion

		#region Collect Loot
		private Action collectLootReceived;
		private bool collect = false;
		
		public void CollectLoot(string enemyName, bool collect, Action collectLootReceived)
		{
			if (Data.Instance.offline)
			{
				collectLootReceived.Invoke();
			}
			else
			{
				this.collectLootReceived = collectLootReceived;
				this.collect = collect;
			
				PlayFabCloudScriptAPI.ExecuteEntityCloudScript(
					new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest()
					{
						Entity = Data.Instance.entityKey, FunctionName = CollectLootFunctionName, GeneratePlayStreamEvent = true,
						FunctionParameter = new {enemy = enemyName, collect = collect}
					}, LootBagReceived, OnLootFailure);
			}
		}
		
		private void LootBagReceived(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
		{
			if (collect)
			{
				if (result.FunctionResult != null)
				{
					// JsonObject jsonResult = (JsonObject) result.FunctionResult;
					// object cardObject;
					// object amountObject;
					// object goldObject;
					// object expObject;
					// object levelObject;
					// jsonResult.TryGetValue("cardAdded", out cardObject);
					// jsonResult.TryGetValue("amountOwned", out amountObject);
					// jsonResult.TryGetValue("goldAdded", out goldObject);
					// jsonResult.TryGetValue("expAdded", out expObject);
					// jsonResult.TryGetValue("playerLevel", out levelObject);
					//
					// string cardAdded = (string) cardObject;
					//
					// int amountAdded = 0;
					// int amountOwned = Convert.ToInt32(amountObject);
					// int goldAdded = Convert.ToInt32(goldObject);
					// int expAdded = Convert.ToInt32(expObject);
					// int playerLevel = Convert.ToInt32(levelObject);
					//
					// bool newItem = false;
					// CardData card;
					// if (Data.instance.collection.inventory.TryGetValue(cardAdded, out card))
					// {
					// 	amountAdded = amountOwned - 1 - card.amountOwned;
					// }
					// else if (Data.instance.collection.allCards.TryGetValue(cardAdded, out card))
					// {
					// 	amountAdded = amountOwned;
					// 	newItem = true;
					// }
					//
					// Data.instance.collection.AddCards(cardAdded, amountAdded);
					//
					// Data.Instance.Currency.gold.ChangeValue(goldAdded);
					//
					// battleEnd.LootReceived(card, amountAdded, goldAdded, expAdded, newItem);
				}
				else
				{
					Debug.LogError("Enemy drop pool does NOT exist.");
				}
			}
			
			collectLootReceived.Invoke();
		}
		
		private void OnLootFailure(PlayFabError error)
		{
			Debug.LogError("CollectLoot FAILURE");
			Debug.LogError("Here's some debug information:");
			Debug.LogError(error.GenerateErrorReport());
		}
		#endregion
	}
}
