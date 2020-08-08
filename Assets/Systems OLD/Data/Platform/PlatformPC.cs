using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;


public class PlatformPC : MonoBehaviour
{
    public void Login(GetPlayerCombinedInfoRequestParams infoRequest, Action<LoginResult> success, Action<PlayFabError> failure)
    {
        PlayFabSettings.TitleId = "AAC4";

        string loginID = PlayerPrefs.GetString("V2Login");

        if (loginID == "")
        {
            loginID = Mathf.Round(UnityEngine.Random.Range(1, 100000000)).ToString();
            PlayerPrefs.SetString("V2Login", loginID);
        }

        var request = new LoginWithCustomIDRequest { CustomId = loginID, CreateAccount = true, InfoRequestParameters = infoRequest };
        PlayFabClientAPI.LoginWithCustomID(request, success, failure);
    }
}
