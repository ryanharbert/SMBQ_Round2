using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.CloudScriptModels;

public class GuildApp : MonoBehaviour {

    public Text nameText;
    public Button acceptButton;
    public Button rejectButton;
    public EntityKey entityKey;
	public int memberCount;

    public void Accept()
    {
		if(memberCount < 40)
		{
			acceptButton.interactable = false;
			rejectButton.interactable = false;
			EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
			PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "guildApplication", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, appKey = entityKey, acceptApp = true } }, AppReturned, ServerFail);
		}
		else
		{
			Warning.instance.Activate("You already have the maximum number of members in your guild");
		}
	}

    public void Reject()
	{
		acceptButton.interactable = false;
		rejectButton.interactable = false;
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "guildApplication", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, appKey = entityKey, acceptApp = false } }, AppReturned, ServerFail);
	}

	public void AppReturned(ExecuteCloudScriptResult result)
	{
		if ((bool)result.FunctionResult)
		{
			gameObject.SetActive(false);
		}
		else
		{
			acceptButton.interactable = true;
			acceptButton.interactable = true;
			Warning.instance.Activate("Error with the application");
		}
	}

    void ServerFail(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }
}
