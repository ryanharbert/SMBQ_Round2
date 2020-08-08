using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.Json;
using PlayFab.CloudScriptModels;

public class Applications : MonoBehaviour
{
    public GameObject loading;
    public GameObject applicationsObject;

    public Toggle applyToggle;
    public Toggle openToggle;

	public Text applyText;
	public Text openText;

	public Color activeToggle;
	public Color inactiveToggle;
    
	public GameObject noApplicationsObject;

    public GuildApp[] apps;

    private void OnEnable()
    {
        applicationsObject.SetActive(false);
		noApplicationsObject.SetActive(false);
        loading.SetActive(true);

        EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
        ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "getGuildApplications", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id } };
        PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, ApplicationsReturned, ListApplicationsFailed);
    }

    void ApplicationsReturned(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object openObject;
            object appDataObject;
			object memberCountObject;
            jsonResult.TryGetValue("Open", out openObject);
            jsonResult.TryGetValue("Applications", out appDataObject);
			jsonResult.TryGetValue("MemberCount", out memberCountObject);

			bool open = (bool)openObject;
            GuildApplicationData[] appData = PlayFabSimpleJson.DeserializeObject<GuildApplicationData[]>(appDataObject.ToString());
			int memberCount = System.Convert.ToInt32(memberCountObject);

            applicationsObject.SetActive(true);
            loading.SetActive(false);

			if(appData.Length > 0)
			{
				noApplicationsObject.SetActive(false);
			}
			else
			{
				noApplicationsObject.SetActive(true);
			}

			bool allowed = Data.instance.guild.Allowed("", "Applications");

            for (int i = 0; i < apps.Length; i++)
            {
                if (i < appData.Length)
                {
                    apps[i].gameObject.SetActive(true);
                    apps[i].entityKey = appData[i].entityKey;
                    apps[i].nameText.text = appData[i].displayName;
					apps[i].memberCount = memberCount;
					if(allowed)
					{
						apps[i].acceptButton.interactable = true;
						apps[i].rejectButton.interactable = true;
					}
					else
					{
						apps[i].acceptButton.interactable = false;
						apps[i].rejectButton.interactable = false;
					}
                }
                else
                {
                    apps[i].gameObject.SetActive(false);
                }

                SetToggles(open);
            }
        }
        else
        {
            Debug.LogError("Oops");
        }
    }

    void SetToggles(bool open)
	{
		if (Data.instance.guild.rank == "Leader")
		{
			applyToggle.interactable = true;
			openToggle.interactable = true;
			applyText.color = activeToggle;
			openText.color = activeToggle;
		}
		else
		{
			applyToggle.interactable = false;
			openToggle.interactable = false;
			applyText.color = inactiveToggle;
			openText.color = inactiveToggle;
		}

		openToggle.onValueChanged.RemoveAllListeners();
        applyToggle.onValueChanged.RemoveAllListeners();
        if (open)
        {
            openToggle.isOn = true;
            applyToggle.isOn = false;
        }
        else
        {
            openToggle.isOn = false;
            applyToggle.isOn = true;
        }
        openToggle.onValueChanged.AddListener(Open);
        applyToggle.onValueChanged.AddListener(Apply);
    }

    public void Apply(bool on)
    {
        if(on)
        {
			EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
			ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "applyOrOpen", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, open = false } };
			PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, ApplyOrOpenReturned, ListApplicationsFailed);

			applyToggle.interactable = false;
            openToggle.interactable = false;
        }
    }

    public void Open(bool on)
    {
        if (on)
		{
			EntityKey e = new EntityKey() { Id = Data.instance.user.entityID, Type = Data.instance.user.entityType };
			ExecuteEntityCloudScriptRequest request = new ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "applyOrOpen", GeneratePlayStreamEvent = true, FunctionParameter = new { guildId = Data.instance.guild.entityKey.Id, open = true } };
			PlayFabCloudScriptAPI.ExecuteEntityCloudScript(request, ApplyOrOpenReturned, ListApplicationsFailed);

			applyToggle.interactable = false;
            openToggle.interactable = false;
        }
    }

    void ApplyOrOpenReturned(ExecuteCloudScriptResult result)
    {
        applyToggle.interactable = true;
        openToggle.interactable = true;
    }

    void ListApplicationsFailed(PlayFabError error)
    {
        Debug.Log(error.ToString());
    }


}
