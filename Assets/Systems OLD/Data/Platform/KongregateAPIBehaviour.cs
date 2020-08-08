using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class KongregateAPIBehaviour : MonoBehaviour
{
    //	public static KongregateAPIBehaviour instance;

    //	public GameObject loginButton;
    //	public GameObject loading;

    //    public Dictionary<string, int> kongOffers;

    //    public string authTicket;
    //    public string kongregateId;
    //    public string username;

    //    void Awake ()
    //	{
    //		instance = this;
    //    }

    //    public void Login()
    //    {
    //        Debug.unityLogger.logEnabled = false;

    //        Debug.Log("Auth Token: " + authTicket);
    //        Debug.Log("Kongregate Id: " + kongregateId);
    //        Debug.Log("Username: " + username);

    //        /* 
    //         * We then execute PlayFab API call called LoginWithKongregate.
    //         * LoginWithKongregate requires KongregateID and AuthTicket. 
    //         * We also pass CreateAccount flag, to automatically create player account.
    //         */


    //        PlayFabSettings.TitleId = "AAC4";

    //        GetPlayerCombinedInfoRequestParams infoRequest = new GetPlayerCombinedInfoRequestParams() { GetUserAccountInfo = true, GetPlayerProfile = true, GetPlayerStatistics = true, GetUserData = true, GetUserReadOnlyData = true, GetTitleData = true, GetUserVirtualCurrency = true, GetUserInventory = true };

    //        PlayFabClientAPI.LoginWithKongregate(new LoginWithKongregateRequest
    //        {
    //            InfoRequestParameters = infoRequest,
    //            KongregateId = kongregateId,
    //            AuthTicket = authTicket,
    //            CreateAccount = true,
    //            TitleId = "aac4"
    //        }, OnLoginSuccess, OnLoginFailure);
    //    }

    //public void KongLogin()
    //{
    //    PlayFabSettings.TitleId = "AAC4";

    //    infoRequest = new GetPlayerCombinedInfoRequestParams() { GetUserAccountInfo = true, GetPlayerProfile = true, GetPlayerStatistics = true, GetUserData = true, GetUserReadOnlyData = true, GetTitleData = true, GetUserVirtualCurrency = true, GetUserInventory = true };

    //    PlayFabClientAPI.LoginWithKongregate(new LoginWithKongregateRequest
    //    {
    //        InfoRequestParameters = infoRequest,
    //        KongregateId = KongregateAPIBehaviour.instance.kongregateId,
    //        AuthTicket = KongregateAPIBehaviour.instance.authTicket,
    //        CreateAccount = true,
    //        TitleId = "aac4"
    //    }, OnLoginSuccess, OnLoginFailure);
    //}

    //    public void KongLogin()
    //    {
    //        Debug.unityLogger.logEnabled = false;
    //        gameObject.name = "KongregateAPI";
    //		DontDestroyOnLoad(this);

    //		Application.ExternalEval(
    //		  @"if(typeof(kongregateUnitySupport) != 'undefined'){
    //        kongregateUnitySupport.initAPI('KongregateAPI', 'OnKongregateAPILoaded');
    //      };"
    //		);
    //	}

    //	public void OnKongregateAPILoaded(string userInfo)
    //	{
    //		// We split userInfo string using '|' character to acquire auth token and Kongregate ID.
    //		var userInfoArray = userInfo.Split('|');
    //		var authTicket = userInfoArray[2];
    //		var kongregateId = userInfoArray[0];
    //		var username = userInfoArray[1];

    //		Debug.Log("Auth Token: " + authTicket);
    //		Debug.Log("Kongregate Id: " + kongregateId);
    //		Debug.Log("Username: " + username);

    //#if UNITY_WEBGL
    //		instance.authTicket = authTicket;
    //		instance.kongregateId = kongregateId;
    //		instance.username = username;
    //#endif

    //		if(username != "Guest" && System.Convert.ToInt32(kongregateId) != 0)
    //		{
    //			Debug.Log("LOGGING IN");
    //			PlayFabLogin.instance.KongLogin();

    //			loginButton.SetActive(false);
    //			loading.SetActive(true);
    //		}
    //		else
    //		{
    //			Debug.Log("WAITING FOR SIGN IN");

    //			loginButton.SetActive(true);
    //			loading.SetActive(false);

    //			Application.ExternalEval(@"
    //			kongregate.services.addEventListener('login', function(){
    //			var unityObject = kongregateUnitySupport.getUnityObject();
    //			var services = kongregate.services;
    //			var params=[services.getUserId(), services.getUsername(), 
    //						services.getGameAuthToken()].join('|');

    //			unityObject.SendMessage('KongregateAPI', 'OnKongregateAPILoaded', params);
    //			});"
    //			);
    //		}
    //	}

    //	public void ShowRegistrationBox()
    //	{
    //		Application.ExternalCall("kongregate.services.showRegistrationBox");
    //	}

    //	public void PurchasePremiumCurrencyOffer(CurrencyOffer currencyOffer)
    //	{
    //		Application.ExternalEval(@"kongregate.mtx.purchaseItems(['" + currencyOffer.offerName + "'], function(result) { var unityObject = kongregateUnitySupport.getUnityObject(); if (result.success) { unityObject.SendMessage('KongregateAPI', 'OnPremiumPurchaseSuccess', ''); } else { unityObject.SendMessage('KongregateAPI', 'OnPremiumPurchaseFailure', '');  } });");
    //	}

    //	public void OnPremiumPurchaseSuccess(string message)
    //	{
    //		OfferConfirmation.instance.OnPremiumPurchaseSuccess(message);
    //	}

    //	public void OnPremiumPurchaseFailure(string message)
    //	{
    //		OfferConfirmation.instance.OnPremiumPurchaseFailure(message);
    //    }


    //    void KongItemListSuccess(ExecuteCloudScriptResult result)
    //	{
    //		if (result.FunctionResult != null)
    //		{
    //			kongOffers = PlayFabSimpleJson.DeserializeObject<Dictionary<string, int>>((string)result.FunctionResult);
    //		}
    //	}

    //    public void GetLoginData(Action callback)
    //    {
    //        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getKongItemList" }, KongItemListSuccess, GetDataFailure);
    //    }
}
