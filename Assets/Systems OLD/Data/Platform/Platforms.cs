using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class Platforms
{
    private PlatformPC pc;
    [SerializeField] private KongregateAPIBehaviour kongregate;

    public GetPlayerCombinedInfoRequestParams infoRequest { get; private set; }

    Action LoginSuccessCallback;
    Action LoginNewUserCallback;
    Action LoginFailureCallback;

    bool waitingForLoginData;

    public void Login(Action LoginSuccessCallback, Action LoginNewUserCallback, Action LoginFailureCallback)
    {
        this.LoginSuccessCallback = LoginSuccessCallback;
        this.LoginNewUserCallback = LoginNewUserCallback;
        this.LoginFailureCallback = LoginFailureCallback;

        infoRequest = new GetPlayerCombinedInfoRequestParams() {
            GetUserAccountInfo = true,
            GetPlayerProfile = true,
            GetPlayerStatistics = true,
            GetUserData = true,
            GetUserReadOnlyData = true,
            GetTitleData = true,
            GetUserVirtualCurrency = true,
            GetUserInventory = true
        };

#if UNITY_EDITOR
        pc = new PlatformPC();
        pc.Login(infoRequest, LoginSuccess, LoginFailure);
#elif UNITY_WEBGL
		KongregateAPIBehaviour.instance.KongLogin(LoginSuccess, LoginFailure);
#else
        pc.Login(infoRequest, LoginSuccess, LoginFailure);
#endif
    }

    void LoginSuccess(LoginResult result)
    {
        Data.instance.user.Login(result);

        UserDataRecord userDataRecord;
        if (result.NewlyCreated == true || !result.InfoResultPayload.UserReadOnlyData.TryGetValue("Tutorial", out userDataRecord))
        {
            LoginNewUserCallback.Invoke();
        }
        else
        {
            GetPlayerCombinedInfoResultPayload playerInfo = result.InfoResultPayload;
            Data.instance.SetLoginData(playerInfo, LoginSuccessCallback);
        }
    }

    void LoginFailure(PlayFabError error)
    {
        LoginFailureCallback.Invoke();
    }

    public IEnumerator GetLoginData()
    {
        waitingForLoginData = false;

#if UNITY_WEBGL
        waitingForLoginData = true;
        kongregate.GetLoginData(LoginDataReceived);
#endif

        while (waitingForLoginData)
        {
            yield return null;
        }
    }

    void LoginDataReceived()
    {
        waitingForLoginData = false;
    }
}
