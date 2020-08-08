using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

[System.Serializable]
public class AllRaidData
{
	public Dictionary<string, RaidData> allRaids;
    public int currentLevel;
    public int raidIndex = 0;
    public List<string> eventRaids;
    public int difficultyIndex = 0;
    public List<int> difficulties;
    public EventData currentEvent;

    public ChestSlotData[] chestSlots;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		string raidDataJson;

		//if (playerInfo.TitleData.TryGetValue("Raidsv2", out raidDataJson))
        if (playerInfo.TitleData.TryGetValue("Raidsv2", out raidDataJson))
        {
			allRaids = PlayFabSimpleJson.DeserializeObject<Dictionary<string, RaidData>>(raidDataJson);
        }

        eventRaids = new List<string>();
        foreach (KeyValuePair<string, RaidData> k in Data.instance.raids.allRaids)
        {
            eventRaids.Add(k.Key);
        }

        difficulties = new List<int>();
        foreach (KeyValuePair<string, string> k in Data.instance.raids.allRaids[CurrentRaidName()].Difficulty)
        {
            difficulties.Add(System.Convert.ToInt32(k.Key));
        }
        currentLevel = difficulties[difficultyIndex];

        string eventJson;
        if (playerInfo.TitleData.TryGetValue("RaidEventv2", out eventJson))
        {
            currentEvent = PlayFabSimpleJson.DeserializeObject<EventData>(eventJson);
        }

        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("ChestSlots", out userDataRecord))
        {
            chestSlots = PlayFabSimpleJson.DeserializeObject<ChestSlotData[]>(userDataRecord.Value);
            foreach (ChestSlotData c in chestSlots)
            {
                SetChestSlot(c);
            }
        }
    }

    public void SetChestSlot(ChestSlotData c)
    {
        if (c.TimeStamp != 0)
        {
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            c.dateTime = d.AddMilliseconds(c.TimeStamp).AddHours(c.UnlockHours);
        }
    }

    public void ChangeIndex(int indexChange)
    {
        raidIndex += indexChange;
        if(raidIndex >= eventRaids.Count)
        {
            raidIndex = 0;
        }
        else if(raidIndex < 0)
        {
            raidIndex = eventRaids.Count - 1;
        }

        difficulties = new List<int>();
        foreach (KeyValuePair<string, string> k in Data.instance.raids.allRaids[CurrentRaidName()].Difficulty)
        {
            difficulties.Add(System.Convert.ToInt32(k.Key));
        }
        if (!difficulties.Contains(currentLevel))
        {
            currentLevel = difficulties[0];
            difficultyIndex = 0;
        }
    }

    public void ChangeDifficulty(int difficultyChange)
    {
        difficultyIndex += difficultyChange;
        if (difficultyIndex >= difficulties.Count)
        {
            difficultyIndex = 0;
        }
        else if (difficultyIndex < 0)
        {
            difficultyIndex = difficulties.Count - 1;
        }
        currentLevel = difficulties[difficultyIndex];
    }

    public string CurrentRaidName()
    {
        return eventRaids[raidIndex];
    }

    public RaidTemplateData CurrentRaidTemplate()
    {
        RaidData r = allRaids[eventRaids[raidIndex]];        
        RaidTemplateData t = r.Templates[r.Difficulty[currentLevel.ToString()]];
        return t;
    }

    public ChestData GetRaidChest(string raidName, int level)
    {
        ChestData c = allRaids[raidName].Chest;
        c.amount = Mathf.RoundToInt((15f * level + 180f) / 6f);
        c.gold = Mathf.RoundToInt(c.amount * 2);
        return c;
    }

    public ChestData CurrentRaidChest()
    {
        return GetRaidChest(CurrentRaidName(), currentLevel);
    }

    public int CurrentEventTier(int raidPoints)
    {
        for (int i = 0; i < currentEvent.Point.Count; i++)
        {
            if (currentEvent.Point[i].Req > raidPoints)
            {
                return i;
            }
        }
        return -1;
    }
}