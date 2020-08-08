using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.Json;
using PlayFab.CloudScriptModels;

public class Permissions : MonoBehaviour
{
	public NavBarToggle officer;
	public NavBarToggle member;
	public NavBarToggle recruit;

	public string rankSelected;

	public Toggle promote;
	public Toggle demote;
	public Toggle kick;
	public Toggle applications;
	public Toggle permissions;
	public Button save;

	public GameObject changesLocked;

	public GameObject displayGroup;
	public GameObject loading;

	private void Start()
	{
		recruit.toggle.isOn = true;
	}

	void Setup()
	{
		promote.isOn = Data.instance.guild.permissions[rankSelected].promote;
		demote.isOn = Data.instance.guild.permissions[rankSelected].demote;
		kick.isOn = Data.instance.guild.permissions[rankSelected].kick;
		applications.isOn = Data.instance.guild.permissions[rankSelected].applications;
		permissions.isOn = Data.instance.guild.permissions[rankSelected].permissions;


		if (Data.instance.guild.Allowed(rankSelected, "Permissions"))
		{
			changesLocked.SetActive(false);

			promote.interactable = true;
			demote.interactable = true;
			kick.interactable = true;
			applications.interactable = true;
			permissions.interactable = true;
			save.interactable = true;
		}
		else
		{
			changesLocked.SetActive(true);

			promote.interactable = false;
			demote.interactable = false;
			kick.interactable = false;
			applications.interactable = false;
			permissions.interactable = false;
			save.interactable = false;
		}
	}

	public void Officer(bool on)
	{
		Toggle(on, officer);
		if (on)
		{
			rankSelected = "Officer";
			Setup();
		}
	}

	public void Member(bool on)
	{
		Toggle(on, member);
		if (on)
		{
			rankSelected = "Member";
			Setup();
		}
	}

	public void Recruit(bool on)
	{
		Toggle(on, recruit);
		if (on)
		{
			rankSelected = "Recruit";
			Setup();
		}
	}

	public void Toggle(bool active, NavBarToggle toggle)
	{
		if (active)
		{
			toggle.toggleText.color = NavBar.instance.highlightColor;
			toggle.toggleText.fontSize = 65;
		}
		else
		{
			toggle.toggleText.color = NavBar.instance.normalColor;
			toggle.toggleText.fontSize = 55;
		}
	}

	public void Save()
	{
		loading.SetActive(true);
		displayGroup.SetActive(false);

		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		object functionParams = new { guildId = Data.instance.guild.entityKey.Id, targetRank = rankSelected, promote = promote.isOn, demote = demote.isOn, kick = kick.isOn, applications = applications.isOn, permissions = permissions.isOn };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "savePermissions", GeneratePlayStreamEvent = true, FunctionParameter = functionParams };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, SaveReturned, SaveFailed);
		
		Data.instance.guild.permissions[rankSelected].promote = promote.isOn;
		Data.instance.guild.permissions[rankSelected].demote = demote.isOn;
		Data.instance.guild.permissions[rankSelected].kick = kick.isOn;
		Data.instance.guild.permissions[rankSelected].applications = applications.isOn;
		Data.instance.guild.permissions[rankSelected].permissions = permissions.isOn;
	}

	void SaveReturned(ExecuteCloudScriptResult result)
	{
		if ((bool)result.FunctionResult)
		{
			loading.SetActive(false);
			displayGroup.SetActive(true);
		}
		else
		{
			Debug.LogError("Oops");
		}
	}

	void SaveFailed(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}


}
