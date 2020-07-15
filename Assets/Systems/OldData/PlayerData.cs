using System;
using PlayFab.ClientModels;

[Serializable]
public class PlayerData
{
    public int level;
    public int exp;


    public void LoginSetPlayer(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        playerInfo.UserVirtualCurrency.TryGetValue("XP", out exp);
    }
}
