using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.CloudScriptModels;

public class GuildOptions : MonoBehaviour
{
    public GameObject applicationsObject;
    public GameObject permissionsObject;

    public NavBarToggle applications;
    public NavBarToggle permissions;

    public Text headerText;

    private void Start()
    {
        applications.toggle.isOn = true;
        permissions.toggle.isOn = false;
    }

    public void Applications(bool on)
    {
        ToggleGameObject(applicationsObject, on, applications);
        if(on)
        {
            headerText.text = "Applications";
        }
    }

    public void Permissions(bool on)
    {
        ToggleGameObject(permissionsObject, on, permissions);
        if (on)
        {
            headerText.text = "Permissions";
        }
    }

    public void ToggleGameObject(GameObject go, bool active, NavBarToggle toggle)
    {
        if (active)
        {
            go.SetActive(true);
            toggle.toggleText.color = NavBar.instance.highlightColor;
            toggle.toggleText.fontSize = 55;
        }
        else
        {
            go.SetActive(false);
            toggle.toggleText.color = NavBar.instance.normalColor;
            toggle.toggleText.fontSize = 50;
        }
    }

    public void LeaveGuildButton()
    {
        Question.instance.SetQuestion("Leave Guild", "Are you sure you want to leave " + Data.instance.guild.name + "?", LeaveGuild);
    }

    public void LeaveGuild()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "leaveGuild", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, LeftGuild, Error);
        Guilds.instance.loading.SetActive(true);
        Guilds.instance.createGuild.SetActive(false);
        Guilds.instance.inGuild.gameObject.SetActive(false);
        Guilds.instance.lookingForGuild.SetActive(false);
    }

	void LeftGuild(ExecuteCloudScriptResult result)
	{
		ChatManager.instance.chatClient.Unsubscribe(new string[] { ChatManager.instance.guildChatRoom });
		ChatManager.instance.guildChatRoom = "";
		Guilds.instance.Setup();
	}

	void Error(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}

}
