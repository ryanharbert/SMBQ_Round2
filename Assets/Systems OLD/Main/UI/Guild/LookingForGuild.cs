using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.Json;
using PlayFab.CloudScriptModels;

public class LookingForGuild : MonoBehaviour
{
	public GameObject loadingObject;
	public GameObject foundGuildsObject;

	public ListedGuild[] foundGuilds;

	string search = "";
	bool searchReturned = false;

	private void OnEnable()
	{
		if(!searchReturned)
		{
			Search();
		}
	}

	public void CreateGuild()
	{
		if(Data.instance.currency.gold >= 5000)
		{
			Guilds.instance.createGuild.SetActive(true);
			gameObject.SetActive(false);
		}
		else
		{
			Warning.instance.Activate("You need 5,000 gold to create a guild");
		}
	}

	public void SearchInput(string search)
	{
		this.search = search;
	}

	public void Search()
	{
		searchReturned = false;
		loadingObject.SetActive(true);
		foundGuildsObject.SetActive(false);
		EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
		ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "findGuilds", GeneratePlayStreamEvent = true, FunctionParameter = new { search = search } };
		PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, GuildsFound, SearchFailed);
	}

	public void GuildsFound(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			loadingObject.SetActive(false);
			foundGuildsObject.SetActive(true);
			searchReturned = true;

			SearchedGuildData[] foundGuildsData = PlayFabSimpleJson.DeserializeObject<SearchedGuildData[]>(result.FunctionResult.ToString());
			for(int i = 0; i < foundGuilds.Length; i++)
			{
				if(i < foundGuildsData.Length)
				{
					foundGuilds[i].gameObject.SetActive(true);
					foundGuilds[i].Set(foundGuildsData[i]);
				}
				else
				{
					foundGuilds[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			Debug.LogError("Guilds Not Found.");
		}
	}

	void SearchFailed(PlayFabError error)
	{
		Debug.Log(error.ToString());
	}
}
