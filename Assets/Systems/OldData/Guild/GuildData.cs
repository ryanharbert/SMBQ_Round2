using PlayFab;
using PlayFab.GroupsModels;
using PlayFab.Json;
using System.Collections.Generic;

[System.Serializable]
public class GuildData
{
	public string name;
	public EntityKey entityKey;
	public string rank;
	public Dictionary<string, PermissionData> permissions;
	public string guildMessage;
	public Dictionary<string, GuildQuestScheduleData[]> guildQuestSchedule;
	public GuildQuestUserData guildQuestUserData;

	public bool Allowed(string target, string action)
	{
		bool targetAllowed = false;

		if (rank == "Officer" && (target == "Member" || target == "Recruit" || target == ""))
		{
			targetAllowed = true;
		}
		else if (rank == "Member" && (target == "Recruit" || target == ""))
		{
			targetAllowed = true;
		}
		else if(rank == "Recruit" && target == "")
		{
			targetAllowed = true;
		}

		if (rank == "Leader")
		{
			return true;
		}
		else if(!targetAllowed)
		{
			return false;
		}
		else
		{
			if (action == "Promote" && Data.instance.guild.permissions[rank].promote)
			{
				return true;
			}
			else if (action == "Demote" && Data.instance.guild.permissions[rank].demote)
			{
				return true;
			}
			else if (action == "Applications" && Data.instance.guild.permissions[rank].applications)
			{
				return true;
			}
			else if (action == "Kick" && Data.instance.guild.permissions[rank].kick)
			{
				return true;
			}
			else if (action == "Permissions" && Data.instance.guild.permissions[rank].permissions)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}

	public void Login(PlayFab.ClientModels.GetPlayerCombinedInfoResultPayload playerInfo)
	{
		string guildQuestScheduleJson;
		if (playerInfo.TitleData.TryGetValue("GuildQuestSchedule", out guildQuestScheduleJson))
		{
			guildQuestSchedule = PlayFabSimpleJson.DeserializeObject<Dictionary<string, GuildQuestScheduleData[]>>(guildQuestScheduleJson);
			foreach (KeyValuePair<string, GuildQuestScheduleData[]> k in guildQuestSchedule)
			{
				//Debug.Log(k.Key);
				//Debug.Log(k.Value.Length);
			}
		}

		PlayFab.ClientModels.UserDataRecord userDataRecord;
		if (playerInfo.UserReadOnlyData.TryGetValue("GuildQuest", out userDataRecord))
		{
			guildQuestUserData = PlayFabSimpleJson.DeserializeObject<GuildQuestUserData>(userDataRecord.Value);
		}
	}
}
