using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialData
{
    public Dictionary<string, bool> steps;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Tutorial", out userDataRecord))
        {
            steps = PlayFabSimpleJson.DeserializeObject<Dictionary<string, bool>>(userDataRecord.Value);
        }
    }

    #region ChooseNewPlayerName
    Action nameSetCallback;
    Action nameTakenCallback;
    Action failureCallback;

    public void SetNewPlayerName(string displayName, Action nameSetCallback, Action nameTakenCallback, Action failureCallback)
    {
        this.nameSetCallback = nameSetCallback;
        this.nameTakenCallback = nameTakenCallback;
        this.failureCallback = failureCallback;

        Data.instance.user.SetNewPlayerName(displayName);
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest() { DisplayName = displayName }, NameSetSuccess, NameSetFailure);
    }

    private void NameSetSuccess(UpdateUserTitleDisplayNameResult result)
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "V2NewPlayer" }, NewPlayerSuccess, NewPlayerNameFailure);
    }

    private void NameSetFailure(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.NameNotAvailable || error.Error == PlayFabErrorCode.ProfaneDisplayName || error.Error == PlayFabErrorCode.UsernameNotAvailable)
        {
            nameTakenCallback.Invoke();
        }
        else
        {
            failureCallback.Invoke();
        }
    }

    private void NewPlayerSuccess(ExecuteCloudScriptResult result)
    {
        var request = new GetPlayerCombinedInfoRequest { InfoRequestParameters = Data.instance.platform.infoRequest };
        PlayFabClientAPI.GetPlayerCombinedInfo(request, GotNewPlayerData, NewPlayerNameFailure);
    }

    private void GotNewPlayerData(GetPlayerCombinedInfoResult result)
    {
        Data.instance.SetLoginData(result.InfoResultPayload, nameSetCallback);
    }

    public void NewPlayerNameFailure(PlayFabError error)
    {
        failureCallback.Invoke();

        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region ChooseStartingFaction
    Action chooseFactionCallback;

    public void ConfirmHero(string faction, Action chooseFactionCallback)
    {
        this.chooseFactionCallback = chooseFactionCallback;

        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "V2FactionSelect", FunctionParameter = new { faction = faction, FrameRate = FrameRate.instance.avgFrameRate }, GeneratePlayStreamEvent = true }, HeroSelectedReturned, ServerFailure);
    }

    void HeroSelectedReturned(ExecuteCloudScriptResult result)
    {

        Debug.Log(result.FunctionResult);
        if (result.FunctionResult != null)
        {
            string[] cards = PlayFabSimpleJson.DeserializeObject<string[]>(result.FunctionResult.ToString());

            for (int i = 0; i < cards.Length; i++)
            {
                Data.instance.collection.AddCards(cards[i], 1);
            }

            Data.instance.collection.deck.heroes[0] = cards[0];
            Data.instance.collection.deck.stronghold = cards[1];
            Data.instance.collection.deck.deck[0] = cards[2];
            Data.instance.collection.deck.deck[1] = cards[3];
            Data.instance.collection.deck.deck[2] = cards[4];
            Data.instance.collection.deck.deck[3] = cards[5];

            Data.instance.collection.heroCollection.Remove(cards[0]);
            Data.instance.collection.strongholdCollection.Remove(cards[1]);
            Data.instance.collection.cardCollection.Remove(cards[2]);
            Data.instance.collection.cardCollection.Remove(cards[3]);
            Data.instance.collection.cardCollection.Remove(cards[4]);
            Data.instance.collection.cardCollection.Remove(cards[5]);

            chooseFactionCallback.Invoke();
        }
    }

    private void ServerFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    public void TutorialFinished(ExecuteCloudScriptResult result)
    {
        Data.instance.tutorial.steps["Finished"] = true;
    }
}
