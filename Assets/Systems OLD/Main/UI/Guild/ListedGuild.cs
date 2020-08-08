using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.CloudScriptModels;
using PlayFab.GroupsModels;

public class ListedGuild : MonoBehaviour {

	public Text guildName;
	public Text memberCount;
	public Text buttonText;
	public SearchedGuildData data;

	bool applied = false;

	public void Set(SearchedGuildData g)
	{
		guildName.text = g.Name;
		memberCount.text = g.members + "/20";
		if(g.members >= 20)
		{
			buttonText.text = "Full";
		}
		else if(g.open)
		{
			buttonText.text = "Join";
		}
		else
		{
			buttonText.text = "Apply";
		}
		buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f);
		data = g;
	}

	public void Button()
	{
		if(data.members >= 20)
		{
			Warning.instance.Activate("This guild is already full");
		}
		else if(data.open)
		{
			Join();
		}
		else
		{
			Apply();
		}
	}

	void Join()
	{
		PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "joinGuild", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = data.EntityId } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, GuildJoined, JoinFailed);
		Guilds.instance.lookingForGuild.SetActive(false);
		Guilds.instance.loading.SetActive(true);
	}

	void Apply()
	{
		if (!applied)
		{
			PlayFab.GroupsModels.EntityKey e = new PlayFab.GroupsModels.EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
			PlayFab.GroupsModels.EntityKey g = new PlayFab.GroupsModels.EntityKey() { Id = data.EntityId, Type = "group" };
			ApplyToGroupRequest request = new ApplyToGroupRequest() { Entity = e, Group = g };
			PlayFabGroupsAPI.ApplyToGroup(request, GuildApplied, JoinFailed);
		}
		else
		{
			Warning.instance.Activate("You already applied to this guild");
		}
	}

	void GuildApplied(ApplyToGroupResponse response)
	{
		applied = true;
		Debug.Log(response.Expires);
		buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0.5f);
	}

	void GuildJoined(ExecuteCloudScriptResult result)
	{
		if (System.Convert.ToBoolean(result.FunctionResult.ToString()))
		{
			Guilds.instance.Setup();
		}
		else
		{
			Guilds.instance.lookingForGuild.SetActive(true);
			Warning.instance.Activate("Error joining guild");
		}
	}

	void JoinFailed(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}
}
