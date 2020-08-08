using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class TravelUI : MonoBehaviour
{
	public Button button;
	public Text buttonHeader;
	public Text buttonAction;
	public Text travelDesc;
	public GameObject travelUIObject;
	public GameObject stillOnNodeButton;


	public void Travel()
	{
		travelUIObject.SetActive(false);
        WorldManager.instance.ToggleWorld(false);

        if (WorldManager.instance.currentInteractiveNode.name == "TutorialTravel")
        {
            Data.instance.tutorial.steps["Finished"] = true;
        }

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "V2TravelNode", GeneratePlayStreamEvent = true, FunctionParameter = new { node = WorldManager.instance.currentInteractiveNode.name } }, TravelReceived, WorldManager.instance.ServerFailure);
    }

	public void StillOnNode()
	{
		travelUIObject.SetActive(false);
		stillOnNodeButton.SetActive(true);
		buttonHeader.text = "Travel";
		buttonAction.text = "Go To";
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(Enable);
	}

	public void Enable()
	{
		travelUIObject.SetActive(true);
		stillOnNodeButton.SetActive(false);
		travelDesc.text = "Do you want to travel to " + ((TravelNode)WorldManager.instance.currentInteractiveNode).travelDescription + "?";
	}

	public void Disable()
	{
		travelUIObject.SetActive(false);
		stillOnNodeButton.SetActive(false);
	}

	void TravelReceived(ExecuteCloudScriptResult result)
	{
		Data.instance.world.Island = result.FunctionResult.ToString();
        Data.instance.world.CurrentPlayerNode = ((TravelNode)WorldManager.instance.currentInteractiveNode).nodeDestination;
        Data.instance.world.PreviousPlayerNode = name;

        if (Data.instance.world.VisitedIslands.Contains(Data.instance.world.Island))
		{
			Data.instance.world.VisitedIslands.Add(Data.instance.world.Island);
		}
		
		SceneLoader.ChangeScenes("WorldMap");
    }

    public void TutorialStepFinished(ExecuteCloudScriptResult result)
    {

    }

    public void GetDataFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
