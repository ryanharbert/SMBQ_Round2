using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;
using System;

namespace SMBQ.Data
{
    public class AsyncPvPBattle
    {
        private Action asyncRewardDisplay;

        public void AsyncBattleComplete(string opponent, bool win, Action rewardDisplay)
        {
            asyncRewardDisplay = rewardDisplay;
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(
                new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest()
                {
                    Entity = Data.instance.entityKey, FunctionName = "endPvPFightv3", GeneratePlayStreamEvent = true,
                    FunctionParameter = new {opponent = opponent, win = win}
                }, PvPAsyncRewards, PvPAsnycFailure);
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

                Data.instance.currency.asyncPoints += points;
                Data.instance.currency.gold += gold;

                //battleEnd.AsyncPvp(win, points, gold, rank);
                asyncRewardDisplay.Invoke();
            }
        }

        private void PvPAsnycFailure(PlayFabError error)
        {
            Debug.LogError("PvPAsnycFailure");
            Debug.LogError("Here's some debug information:");
            Debug.LogError(error.GenerateErrorReport());
        }
    }
}
