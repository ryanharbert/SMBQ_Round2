using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class EventRewardsData
{
    public string SeasonReward;
    public string AsyncSeasonReward;
    public DateTime SeasonEnd;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("EventRewards", out userDataRecord))
        {
            JsonUtility.FromJsonOverwrite(userDataRecord.Value, this);
        }

        string seasonEndData;
        if (playerInfo.TitleData.TryGetValue("SeasonEnd", out seasonEndData))
        {
            long seasonEndLong = System.Convert.ToInt64(seasonEndData);
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            SeasonEnd = d.AddMilliseconds(seasonEndLong);
        }
    }

}
