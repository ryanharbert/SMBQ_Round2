using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.CloudScriptModels;

public class CreateGuild : MonoBehaviour
{
	public bool open = true;
	public string guildName = "";

	public void GuildNameInput(string guildName)
	{
		this.guildName = guildName;
	}

	public void Open(bool open)
	{
		this.open = open;
	}

	public void CreateGuildButton()
	{
		if (Data.instance.currency.gold < 5000)
		{
			Warning.instance.Activate("You need 5,000 gold to start a guild");
		}
		else if(guildName.Length < 4)
		{
			Warning.instance.Activate("Guild name must be more than 3 characters");
		}
		else
		{
			CreateGroup();
		}
	}
	
	void CreateGroup()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "createGuild", GeneratePlayStreamEvent = true, FunctionParameter = new { open = open, guildName = guildName } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, GroupCreated, GroupCreateFailed);
	}

	public void GroupCreated(ExecuteCloudScriptResult response)
	{
        Data.instance.currency.gold -= 5000;
		Guilds.instance.Setup();
	}

	void GroupCreateFailed(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}

	public void Cancel()
	{
		Guilds.instance.lookingForGuild.SetActive(true);
		gameObject.SetActive(false);
	}
}
