using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using System;

namespace SMBQ.Data
{
    public class AsyncPvPBattle
    {
        private const string AsyncBattleCompleteFunctionName = "endPvPFightv3";
        
        private Action asyncRewardDisplay;

        public void AsyncBattleComplete(string opponent, bool win, Action rewardDisplay)
        {
            if (Data.Instance.offline)
            {
                Reward(10, 10, 1);
            }
            else
            {
                asyncRewardDisplay = rewardDisplay;
                PlayFabCloudScriptAPI.ExecuteEntityCloudScript(
                    new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest()
                    {
                        Entity = Data.Instance.entityKey, FunctionName = AsyncBattleCompleteFunctionName, GeneratePlayStreamEvent = true,
                        FunctionParameter = new {opponent = opponent, win = win}
                    }, PvPAsyncRewards, AsyncBattleCompleteFailure);
            }
        }

        private void PvPAsyncRewards(PlayFab.CloudScriptModels.ExecuteCloudScriptResult result)
        {
            if (result.FunctionResult != null)
            {
                JsonObject jsonResult = (JsonObject) result.FunctionResult;
                object pointsObject;
                object goldObject;
                object rankObject;
                jsonResult.TryGetValue("points", out pointsObject);
                jsonResult.TryGetValue("gold", out goldObject);
                jsonResult.TryGetValue("rank", out rankObject);

                int points = PlayFabSimpleJson.DeserializeObject<int>((string) pointsObject);
                int gold = PlayFabSimpleJson.DeserializeObject<int>((string) goldObject);
                int rank = PlayFabSimpleJson.DeserializeObject<int>((string) rankObject);

                bool win = true;

                if (points > 5)
                {
                    win = true;
                }
                else
                {
                    win = false;
                }
                
                Reward(points, gold, rank);
            }
        }

        void Reward(int points, int gold, int rank)
        {

            Data.Instance.Currency.asyncPoints += points;
            Data.Instance.Currency.gold += gold;

            //battleEnd.AsyncPvp(win, points, gold, rank);
            asyncRewardDisplay.Invoke();
        }

        private void AsyncBattleCompleteFailure(PlayFabError error)
        {
            Debug.LogError("AsyncBattleComplete FAILURE");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}
