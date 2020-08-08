using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;


public class TutorialWorldMap : MonoBehaviour
{
	public static TutorialWorldMap instance;

	public GameObject clickOnDeck;
	public GameObject clickOnArchers;
	public GameObject archerArrow;
	public GameObject clickOnLevelUp;
	public GameObject clickOnUse;
	
	void Awake ()
	{
		if (!Data.instance.tutorial.steps["CardProfile"])
		{
			instance = this;
			clickOnDeck.SetActive(true);
		}
	}

	public void DeckEditor()
	{
		clickOnArchers.SetActive(true);
		archerArrow.SetActive(true);
		clickOnDeck.SetActive(false);
	}

	public void LevelUp()
	{
		clickOnLevelUp.SetActive(true);
		clickOnArchers.SetActive(false);
		archerArrow.SetActive(false);

		TutorialFinished();
	}

	public void Use()
	{
		clickOnLevelUp.SetActive(false);
		clickOnUse.SetActive(true);
	}

	public void DisableTutorial()
	{
		clickOnDeck.SetActive(false);
		clickOnArchers.SetActive(false);
		clickOnLevelUp.SetActive(false);
		clickOnUse.SetActive(false);
		archerArrow.SetActive(false);
		Data.instance.tutorial.steps["CardProfile"] = true;
		instance = null;
	}

	public void TutorialFinished()
	{
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "tutorialUpgradeFinishedv2", FunctionParameter = new { FrameRate = FrameRate.instance.avgFrameRate } }, TutorialStepFinished, GetDataFailure);
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
