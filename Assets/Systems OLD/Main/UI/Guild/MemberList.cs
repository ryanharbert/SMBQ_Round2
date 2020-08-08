using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.Json;
using PlayFab.CloudScriptModels;

public class MemberList : MonoBehaviour
{
	public Text memberCountText;

	public GuildMember[] memberList;

	public GameObject loadingObject;
	public GameObject memberListObject;

	public Color memberColor;
	public Color yourColor;

	private void OnEnable()
	{
		loadingObject.SetActive(true);
		memberListObject.SetActive(false);


		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "guildMemberList", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, MembersReturned, ListMembersFailed);
	}

	void MembersReturned(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			GuildMemberData[] membersData = PlayFabSimpleJson.DeserializeObject<GuildMemberData[]>(result.FunctionResult.ToString());

			loadingObject.SetActive(false);
			memberListObject.SetActive(true);

			memberCountText.text = "Members: " + membersData.Length + "/20";

			for (int i = 0; i < memberList.Length; i++)
			{
				if (i < membersData.Length)
				{
					memberList[i].gameObject.SetActive(true);
					memberList[i].data = membersData[i];
					memberList[i].displayNameText.text = membersData[i].displayName;
					memberList[i].rankText.text = membersData[i].rankName;


					DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
					d = d.AddMilliseconds(membersData[i].lastLogin);
					TimeSpan t = d - DateTime.UtcNow;
					if (t.TotalDays >= 7)
					{
						memberList[i].lastLoginText.text = -Mathf.CeilToInt((float)(t.TotalDays / 7)) + " Weeks";
					}
					else if (t.Days != 0)
					{
						memberList[i].lastLoginText.text = -t.Days + " Days";
					}
					else if (t.Hours != 0)
					{
						memberList[i].lastLoginText.text = -t.Hours + " Hours";
					}
					else if (t.Minutes != 0)
					{
						memberList[i].lastLoginText.text = -t.Minutes + " Minutes";
					}
					else
					{
						memberList[i].lastLoginText.text = "Just Now";
					}


					if (membersData[i].displayName != Data.instance.user.displayName)
					{
						memberList[i].displayNameText.color = memberColor;
						memberList[i].lastLoginText.color = memberColor;
						memberList[i].rankText.color = memberColor;
					}
					else
					{
						memberList[i].displayNameText.color = yourColor;
						memberList[i].lastLoginText.color = yourColor;
						memberList[i].rankText.color = yourColor;
					}
				}
				else
				{
					memberList[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			Debug.LogError("Oops");
		}
	}

	void ListMembersFailed(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}
}
