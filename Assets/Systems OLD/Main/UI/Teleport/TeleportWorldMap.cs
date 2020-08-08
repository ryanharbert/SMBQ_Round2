using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class TeleportWorldMap : MonoBehaviour
{
	public static TeleportWorldMap instance;

    public GameObject displayObject;

    public Text description;
    public Text timerText;

    public TeleportTarget[] islands;

    public GameObject loading;
    public Button closeButton;

    bool onCooldown;
    DateTime unlockTime;
	string destination;

	private void Start()
	{
		instance = this;
	}

	private void OnEnable()
    {
        SetTeleport();
    }

	void SetTeleport()
    {
        description.text = "Teleport to the beginning of the selected island?";
        timerText.text = "";

		for(int i = 0; i < islands.Length; i++)
		{
			if(i < Data.instance.world.VisitedIslands.Count)
			{
				islands[i].button.onClick.RemoveAllListeners();
				islands[i].button.onClick.AddListener(islands[i].TeleportButton);
				islands[i].buttonImage.color = new Color32(255, 255, 255, 255);
				islands[i].lockObject.SetActive(false);
			}
			else
			{
				islands[i].button.onClick.RemoveAllListeners();
				islands[i].buttonImage.color = new Color32(255, 255, 255, 90);
				islands[i].lockObject.SetActive(true);
			}
		}
    }

    public void Teleport(string island)
    {
		timerText.gameObject.SetActive(false);
		closeButton.gameObject.SetActive(false);
		for (int i = 0; i < islands.Length; i++)
		{
			islands[i].gameObject.SetActive(false);
		}
		loading.SetActive(true);

		destination = island;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "teleport", FunctionParameter = new { destination = island } }, TeleportSuccess, TeleportFailure);
    }

    private void TeleportSuccess(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object nodeObject;
            jsonResult.TryGetValue("node", out nodeObject);

            string node = (string)nodeObject;

			Data.instance.world.Island = destination;
            Data.instance.world.CurrentPlayerNode = node;
            Data.instance.world.PreviousPlayerNode = node;

            SceneLoader.ChangeScenes("WorldMap");
        }
    }

    private void TeleportFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

    private void Update()
    {
        if((unlockTime - DateTime.UtcNow).TotalSeconds <= 0)
        {
            SetTeleport();
        }
        else
        {
            TimeSpan timeSpan = unlockTime - DateTime.UtcNow;

            string timer = "";
            if (timeSpan.Hours != 0)
            {
                timer += timeSpan.Hours + "h ";
            }
            if (timeSpan.Minutes != 0)
            {
                timer += timeSpan.Minutes + "m ";
            }
            if (timeSpan.Seconds != 0)
            {
                timer += timeSpan.Seconds + "s ";
            }
            timerText.text = timer;
        }
    }
}
