using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[Serializable]
public class QuestDataManager
{
    public List<QuestData> quests { get; private set; }
    public DateTime lastQuestTimeStamp { get; private set; }

    Action collectQuestRewardCallback;
    Action<List<int>> getNewQuestsCallback;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Quests", out userDataRecord))
        {
            SetData(userDataRecord.Value);
        }
    }

    public bool TimeForNewQuests(out TimeSpan t)
    {
        if (lastQuestTimeStamp.AddDays(1) > DateTime.UtcNow)
        {
            t = lastQuestTimeStamp.AddDays(1) - DateTime.UtcNow;
            return false;
        }
        return true;
    }

    void SetData(string json)
    {
        Dictionary<string, object> dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(json);
        long timeStamp = Convert.ToInt64(dict["Date"]);
        lastQuestTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp);
        quests = PlayFabSimpleJson.DeserializeObject<List<QuestData>>(dict["Quests"].ToString());
    }

    #region GetNewQuests
    public void GetNewQuests(Action<List<int>> getNewQuestsCallback)
    {
        this.getNewQuestsCallback = getNewQuestsCallback;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getNewQuests", FunctionParameter = new { level = Data.instance.currency.playerLevel } }, GetNewQuestsSuccess, GetNewQuestsFailure);
    }

    void GetNewQuestsSuccess(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object questObject;
            object newQuestsObject;
            jsonResult.TryGetValue("questData", out questObject);
            jsonResult.TryGetValue("newQuests", out newQuestsObject);

            SetData((string)questObject);

            List<int> newQuests = new List<int>();
            if (newQuestsObject != null)
            {
                newQuests = PlayFabSimpleJson.DeserializeObject<List<int>>((string)newQuestsObject);
            }
            getNewQuestsCallback.Invoke(newQuests);
        }
        else
        {
            Debug.LogError("Quests were not returned.");
        }
    }

    void GetNewQuestsFailure(PlayFabError error)
    {
        Debug.LogError("Error getting new quests.");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region CollectQuestReward
    public void CollectQuestReward(int index, Action callback)
    {
        collectQuestRewardCallback = callback;
        quests.RemoveAt(index);

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "collectQuestReward", FunctionParameter = new { index = index } }, CollectQuestRewardSuccess, CollectQuestRewardFailure);
    }

    void CollectQuestRewardSuccess(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            collectQuestRewardCallback.Invoke();
        }
        else
        {
            Debug.LogError("Reward not returned.");
        }
    }

    void CollectQuestRewardFailure(PlayFabError error)
    {
        Debug.LogError("Error collecting quest reward.");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion
}
