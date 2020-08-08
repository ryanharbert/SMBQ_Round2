using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.Json;

public class GuildQuestDisplay : QuestDisplay
{
	public bool set = false;

    public Text newQuestTimerText;
    public GameObject questObject;
	public Text questTimerText;
	public Text tierText;

	DateTime questChangeDate;
	QuestData[] quests;
	int month;
	int day;
	DateTime eventChange;
	GuildQuestScheduleData currentEvent;
	int currentTier;


    public void Set(string json)
	{
		set = false;
		if(json != "")
		{
			quests = PlayFabSimpleJson.DeserializeObject<QuestData[]>(json);
		}
		else
		{
			quests = null;
		}

		month = DateTime.UtcNow.Month;
		day = DateTime.UtcNow.Day;
		GuildQuestScheduleData[] events = Data.instance.guild.guildQuestSchedule[month.ToString()];
		
		if (day < events[0].Start)
		{
			eventChange = new DateTime(DateTime.UtcNow.Year, month, events[0].Start, 0, 0, 0, DateTimeKind.Utc);
			currentEvent = Data.instance.guild.guildQuestSchedule[DateTime.UtcNow.AddMonths(-1).Month.ToString()][Data.instance.guild.guildQuestSchedule[DateTime.UtcNow.AddMonths(-1).Month.ToString()].Length - 1];
			QuestInactive();
		}
		else if(day >= events[events.Length - 1].End)
		{
			eventChange = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.AddMonths(1).Month, Data.instance.guild.guildQuestSchedule[DateTime.UtcNow.AddMonths(1).Month.ToString()][0].Start, 0, 0, 0, DateTimeKind.Utc);
			currentEvent = Data.instance.guild.guildQuestSchedule[month.ToString()][events.Length - 1];
			QuestInactive();
		}
		else
		{
			for (int i = 0; i < events.Length; i++)
			{
				if (events[i].Start <= DateTime.UtcNow.Day && events[i].End > DateTime.UtcNow.Day)
				{
					eventChange = new DateTime(DateTime.UtcNow.Year, month, events[i].End, 0, 0, 0, DateTimeKind.Utc);
					currentEvent = events[i];
					QuestActive();
					break;
				}
				else if(day >= events[i].End && day < events[i + 1].Start)
				{
					eventChange = new DateTime(DateTime.UtcNow.Year, month, events[i + 1].Start, 0, 0, 0, DateTimeKind.Utc);
					currentEvent = events[i];
					QuestInactive();
					break;
				}
			}
		}
    }

	void QuestActive()
	{
		if (Data.instance.guild.guildQuestUserData == null || Data.instance.guild.guildQuestUserData.Target != currentEvent.Target || (Data.instance.guild.guildQuestUserData.Start != currentEvent.Start && Data.instance.guild.guildQuestUserData.Month != DateTime.UtcNow.Month))
		{
			SetGuildQuestUserData();
			return;
		}
		else if(quests == null || currentEvent.Target != quests[0].Target)
		{
			SetGuildQuestObject();
			return;
		}
		else
		{
			for (int i = 0; i < quests.Length; i++)
			{
				if (quests[i].Progress < quests[i].Complete || (i + 1) == quests.Length || !Data.instance.guild.guildQuestUserData.Collected[i])
				{
					currentTier = i;
					Set(quests[i]);
					tierText.text = "Tier " + (i + 1) + " / " + quests.Length;
					if((i + 1) == quests.Length && Data.instance.guild.guildQuestUserData.Collected[i])
					{
						//collectButton.SetActive(false);
						//progressObject.SetActive(true);
						//questProgressText.text = "Complete";
					}
					break;
				}
			}

			newQuestTimerText.gameObject.SetActive(false);
			questObject.SetActive(true);
			set = true;
		}
	}

	void QuestInactive()
	{
		if(quests != null && Data.instance.guild.guildQuestUserData != null && currentEvent.Target == Data.instance.guild.guildQuestUserData.Target)
		{
			for (int i = 0; i < Data.instance.guild.guildQuestUserData.Collected.Length; i++)
			{
				if (!Data.instance.guild.guildQuestUserData.Collected[i] && quests[i].Progress > quests[i].Complete)
				{
					QuestActive();
					return;
				}
			}
		}

		newQuestTimerText.gameObject.SetActive(true);
		questObject.SetActive(false);
		set = true;
	}

    private void Update()
    {
        if(newQuestTimerText.gameObject.activeSelf)
        {
            TimeSpan t = eventChange - DateTime.UtcNow;
            newQuestTimerText.text = "New Guild Quest in: " + TimeSpanDisplay.Format(t);
        }
		else if(currentEvent != null && currentEvent.End > DateTime.UtcNow.Day)
		{
			TimeSpan t = eventChange - DateTime.UtcNow;
			questTimerText.text = TimeSpanDisplay.Format(t) + " left";
		}
		else
		{
			questTimerText.text = "No Time Left";
		}
	}

	public override void CollectRewards()
	{
		//collectButton.SetActive(false);

		Data.instance.guild.guildQuestUserData.Collected[currentTier] = true;
		OfferConfirmation.instance.QuestReward(quests[currentTier].Reward);
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest() { FunctionName = "collectGuildQuest", GeneratePlayStreamEvent = true }, RewardsReceived, GetDataFailure);
	}

	public void RewardsReceived(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			int gold = System.Convert.ToInt32(result.FunctionResult);

			OfferConfirmation.instance.QuestRewardReceived();

			QuestActive();
		}
		else
		{
			Debug.LogError("Reward not returned.");
		}
	}

	void SetGuildQuestUserData()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "setGuildQuestUserData", GeneratePlayStreamEvent = true }, UserDataSet, GetDataFailure);
	}

	private void UserDataSet(ExecuteCloudScriptResult result)
	{
		Data.instance.guild.guildQuestUserData = new GuildQuestUserData();
		Data.instance.guild.guildQuestUserData.Month = month;
		Data.instance.guild.guildQuestUserData.Start = currentEvent.Start;
		Data.instance.guild.guildQuestUserData.Target = currentEvent.Target;
		Data.instance.guild.guildQuestUserData.Collected = new bool[currentEvent.Amounts.Length];
		for(int i = 0; i < Data.instance.guild.guildQuestUserData.Collected.Length; i++)
		{
			Data.instance.guild.guildQuestUserData.Collected[i] = false;
		}
		Guilds.instance.Setup();
	}

	void SetGuildQuestObject()
	{
		EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "setGuildQuestObject", GeneratePlayStreamEvent = true }, GuildDataSet, GetDataFailure);
	}

	private void GuildDataSet(ExecuteCloudScriptResult result)
	{
		Guilds.instance.Setup();
	}

	public void GetDataFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}
}
