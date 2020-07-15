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
}
