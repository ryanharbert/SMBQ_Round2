using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

[Serializable]
public class CurrencyData
{
	public int gold = 0;
	public int gems = 0;
	public int energy = 0;
	public int energyMax = 0;
	public float secondsToRecharge = 0f;
	public float startingEnergySec = 0f;
	public DateTime energyTS;
	public int energyIncreases = 0;
    public int scrolls = 0;
    public int scrollsMax = 0;
    public float scrollsSecToRecharge = 0f;
	public float startingScrollSec = 0f;
	public DateTime scrollsTS;
	public int scrollIncreases = 0;

    public int stars;

	public int raidPoints;
    public int asyncPoints;
    public int exp;
    public int playerLevel;

	public void UpdateEnergyRecharge()
	{
		if (energy < energyMax)
		{
			TimeSpan t = DateTime.UtcNow - energyTS;
			float f = startingEnergySec + (float)t.TotalSeconds;
			if(Mathf.FloorToInt(f / 3600) > energyIncreases)
			{
				energy += 1;
				energyIncreases++;
			}
			secondsToRecharge = Mathf.CeilToInt(3600 - (f % 3600));
		}
	}

	public void UpdateScrollsRecharge()
	{
		if (scrolls < scrollsMax)
		{
			TimeSpan t = DateTime.UtcNow - scrollsTS;
			float f = startingScrollSec + (float)t.TotalSeconds;
			if (Mathf.FloorToInt(f / 2880) > scrollIncreases)
			{
				scrolls += 1;
				scrollIncreases++;
			}
			scrollsSecToRecharge = Mathf.CeilToInt(2880 - (f % 2880));
		}
	}

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		playerInfo.UserVirtualCurrency.TryGetValue("GO", out gold);
		playerInfo.UserVirtualCurrency.TryGetValue("DI", out gems);
		playerInfo.UserVirtualCurrency.TryGetValue("LB", out energy);
        energy -= 1;
        playerInfo.UserVirtualCurrency.TryGetValue("SC", out scrolls);
        scrolls -= 1;
        playerInfo.UserVirtualCurrency.TryGetValue("SP", out stars);
        playerInfo.UserVirtualCurrency.TryGetValue("XP", out exp);
        playerInfo.UserVirtualCurrency.TryGetValue("LV", out playerLevel);
        VirtualCurrencyRechargeTime rechargeTime;
		playerInfo.UserVirtualCurrencyRechargeTimes.TryGetValue("LB", out rechargeTime);
		energyMax = rechargeTime.RechargeMax - 1;
		startingEnergySec = 3600 - rechargeTime.SecondsToRecharge;
		energyTS = DateTime.UtcNow;
        VirtualCurrencyRechargeTime scrollRechargeTime;
        playerInfo.UserVirtualCurrencyRechargeTimes.TryGetValue("SC", out scrollRechargeTime);
        scrollsMax = scrollRechargeTime.RechargeMax - 1;
		startingScrollSec = 2880 - scrollRechargeTime.SecondsToRecharge;
		scrollsTS = DateTime.UtcNow;

		raidPoints = 0;
        asyncPoints = 0;

        foreach(StatisticValue s in playerInfo.PlayerStatistics)
        {
            if(s.StatisticName == "AsyncPoints")
            {
                asyncPoints = s.Value;
            }
            else if(s.StatisticName == "RaidPoints")
            {
                raidPoints = s.Value;
            }
        }
	}

    public void IncreaseExp(int expAdded, int newLevel)
    {
        exp += expAdded;
        if (playerLevel != newLevel)
        {
            exp = exp - Data.instance.values.GetLevelUpCost(playerLevel);
            playerLevel = newLevel;
        }
    }
}
