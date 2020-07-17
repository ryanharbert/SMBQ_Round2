using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialData
{
    public Dictionary<string, bool> steps;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Tutorial", out userDataRecord))
        {
            steps = PlayFabSimpleJson.DeserializeObject<Dictionary<string, bool>>(userDataRecord.Value);
        }
    }
}
