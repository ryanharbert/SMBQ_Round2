using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.CloudScriptModels;

public class GuildMemberProfile : MonoBehaviour
{
	public static GuildMemberProfile instance;

	public GameObject displayObject;

	public Text displayNameText;
	public Text lastLoginText;
	public Text rankText;
	public Button promoteButton;
	public Button demoteButton;
	public Button kickButton;
	public Image promoteImage;
	public Image demoteImage;
	public Image kickImage;

	string action;
	GuildMember member;


	private void Start()
	{
		instance = this;
	}

	public void Set(GuildMember member)
	{
		this.member = member;

		displayObject.SetActive(true);

		displayNameText.text = member.displayNameText.text;
		lastLoginText.text = member.lastLoginText.text;
		rankText.text = member.rankText.text;
		if(member.data.rankName != "Officer")
		{
			SetButton(promoteImage, promoteButton, Data.instance.guild.Allowed(member.data.rankName, "Promote"));
		}
		else
		{
			SetButton(promoteImage, promoteButton, false);
		}
		if (member.data.rankName != "Recruit")
		{
			SetButton(demoteImage, demoteButton, Data.instance.guild.Allowed(member.data.rankName, "Demote"));
		}
		else
		{
			SetButton(demoteImage, demoteButton, false);
		}
		SetButton(kickImage, kickButton, Data.instance.guild.Allowed(member.data.rankName, "Kick"));

        if(member.displayNameText.text == Data.instance.user.displayName)
        {
            SetButton(promoteImage, promoteButton, false);
            SetButton(demoteImage, demoteButton, false);
            SetButton(kickImage, kickButton, false);
        }
	}

	void SetButton(Image i, Button b, bool enabled)
	{
		b.interactable = enabled;
		if(enabled)
		{
			i.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			i.color = new Color(1f, 1f, 1f, 0.5f);
		}
	}

	public void Promote()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "memberChange", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, appKey = member.data.entityKey, targetRank = member.data.rankName, aaction = "Promote" } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, PlayerChangeReturned, Error);
		action = "Promote";

		promoteButton.interactable = false;
		demoteButton.interactable = false;
		kickButton.interactable = false;
	}

	public void Demote()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "memberChange", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, appKey = member.data.entityKey, targetRank = member.data.rankName, aaction = "Demote" } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, PlayerChangeReturned, Error);
		action = "Demote";

		promoteButton.interactable = false;
		demoteButton.interactable = false;
		kickButton.interactable = false;
	}

	public void Kick()
	{
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "memberChange", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, appKey = member.data.entityKey, targetRank = member.data.rankName, aaction = "Kick" } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, PlayerChangeReturned, Error);
		action = "Kick";

		promoteButton.interactable = false;
		demoteButton.interactable = false;
		kickButton.interactable = false;
	}

	void PlayerChangeReturned(ExecuteCloudScriptResult result)
	{
		if ((bool)result.FunctionResult)
		{
			promoteButton.interactable = true;
			demoteButton.interactable = true;
			kickButton.interactable = true;

			if (action == "Promote")
			{
				if(member.rankText.text == "Recruit")
				{
					member.data.rankName = "Member";
					member.rankText.text = "Member";
				}
				else if (member.rankText.text == "Member")
				{
					member.data.rankName = "Officer";
					member.rankText.text = "Officer";
				}
			}
			else if(action == "Demote")
			{
				if (member.rankText.text == "Officer")
				{
					member.data.rankName = "Member";
					member.rankText.text = "Member";
				}
				else if (member.rankText.text == "Member")
				{
					member.data.rankName = "Recruit";
					member.rankText.text = "Recruit";
				}
			}
			else
			{
				member.gameObject.SetActive(false);
			}
			displayObject.SetActive(false);
		}
		else
		{
			Debug.LogError("Oops");
		}
	}

	void Error(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}
}
