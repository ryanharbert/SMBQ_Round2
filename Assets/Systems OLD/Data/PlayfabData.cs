using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[Serializable]
public class PlayfabData
{
    public string displayName { get; private set; }
    public string playfabID { get; private set; }
    public string entityID { get; private set; }
    public string entityType { get; private set; }

    public void Login(LoginResult result)
    {
        displayName = result.InfoResultPayload.PlayerProfile.DisplayName;
        playfabID = result.InfoResultPayload.AccountInfo.PlayFabId;
        entityID = result.EntityToken.Entity.Id;
        entityType = result.EntityToken.Entity.Type;
    }

    public void SetNewPlayerName(string displayName)
    {
        this.displayName = displayName;
    }
}
