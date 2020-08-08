using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.DataModels;
using PlayFab.GroupsModels;
using PlayFab.Json;

public class Guilds : MonoBehaviour
{
    public static Guilds instance;

	public GameObject lookingForGuild;
	public InGuild inGuild;
    public GameObject createGuild;
    public GameObject loading;

	private void Start()
	{
        instance = this;
    }

    private void OnEnable()
    {
		Setup();
    }

	public void Setup()
	{
		loading.SetActive(true);
		createGuild.SetActive(false);
		inGuild.gameObject.SetActive(false);
		lookingForGuild.SetActive(false);
		PlayFab.GroupsModels.EntityKey e = new PlayFab.GroupsModels.EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ListMembershipRequest request = new ListMembershipRequest() { Entity = e };
		PlayFabGroupsAPI.ListMembership(request, MembershipResponse, ServerFailure);
	}

    public void MembershipResponse(ListMembershipResponse response)
    {
        if (response.Groups.Count > 0)
        {
			if(ChatManager.instance.guildChatRoom == "")
			{
				ChatManager.instance.guildChatRoom = response.Groups[0].GroupName;
				ChatManager.instance.chatClient.Subscribe(new string[] { ChatManager.instance.guildChatRoom });
			}

			Data.instance.guild.name = response.Groups[0].GroupName;
			Data.instance.guild.entityKey = response.Groups[0].Group;
			Data.instance.guild.rank = response.Groups[0].Roles[0].RoleName;

			PlayFab.DataModels.EntityKey e = new PlayFab.DataModels.EntityKey() { Id = Data.instance.guild.entityKey.Id, Type = Data.instance.guild.entityKey.Type };
			PlayFabDataAPI.GetObjects(new GetObjectsRequest() { Entity = e }, ReceivedGuildObjects, ServerFailure);
		}
        else
		{
			loading.SetActive(false);
			lookingForGuild.SetActive(true);
        }
    }

	void ReceivedGuildObjects(GetObjectsResponse response)
	{
		Data.instance.guild.permissions = PlayFabSimpleJson.DeserializeObject<Dictionary<string, PermissionData>>(response.Objects["Permissions"].DataObject.ToString());

		ObjectResult message;
		if(response.Objects.TryGetValue("Message", out message))
		{
			JsonObject jsonResult = (JsonObject)message.DataObject;
			object messageObject;
			jsonResult.TryGetValue("Message", out messageObject);

			Data.instance.guild.guildMessage = messageObject.ToString();
		}
		else
		{
			Data.instance.guild.guildMessage = "";
		}

		ObjectResult quests;
		if (response.Objects.TryGetValue("Quests", out quests))
		{
			inGuild.guildQuest.Set(quests.DataObject.ToString());
		}
		else
		{
			inGuild.guildQuest.Set("");
		}

		StartCoroutine("SetChatChannel");
	}

	IEnumerator SetChatChannel()
	{
		while (!ChatManager.instance.guildSubscribed || !inGuild.guildQuest.set)
		{
			yield return null;
		}

		loading.SetActive(false);
		inGuild.gameObject.SetActive(true);
	}

	void ServerFailure(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }
}
