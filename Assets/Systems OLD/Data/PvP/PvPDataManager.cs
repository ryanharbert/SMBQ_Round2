using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class PvPDataManager
{
    public BattleData pvpBattle;
    public EventData pvpEvent;
    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        string eventJson;
        if (playerInfo.TitleData.TryGetValue("PvPEventv2", out eventJson))
        {
            pvpEvent = PlayFabSimpleJson.DeserializeObject<EventData>(eventJson);
        }
    }

    public int CurrentPvPEventTier(int asyncPoints)
    {
        for (int i = 0; i < pvpEvent.Point.Count; i++)
        {
            if (pvpEvent.Point[i].Req > asyncPoints)
            {
                return i;
            }
        }
        return -1;
    }
}
