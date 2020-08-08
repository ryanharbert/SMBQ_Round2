using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class ShopDataManager
{
	public int month;
	public int day;
    public Dictionary<string, ShopData> shops;
    public string currentShop;
    public GetCatalogItemsResult currencyOffers;

    bool waitingForLoginData;

    public IEnumerator Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        string shopsJson;

        if (playerInfo.TitleData.TryGetValue("Shops", out shopsJson))
        {
            shops = PlayFabSimpleJson.DeserializeObject<Dictionary<string, ShopData>>(shopsJson);
        }

        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Shops", out userDataRecord))
        {
            Dictionary<string, object> userShops = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(userDataRecord.Value);

            object monthObject;
            if(userShops.TryGetValue("Month", out monthObject))
            {
                month = System.Convert.ToInt32(monthObject);
            }
            object dayObject;
            if (userShops.TryGetValue("Day", out dayObject))
            {
                day = System.Convert.ToInt32(dayObject);
            }
            object purchasedObject;
            if (userShops.TryGetValue("Shops", out purchasedObject))
            {                
                Dictionary<string, List<bool>> userPurchased = PlayFabSimpleJson.DeserializeObject<Dictionary<string, List<bool>>>(purchasedObject.ToString());
                foreach(KeyValuePair<string, ShopData> k in shops)
                {
                    k.Value.Purchased = userPurchased[k.Key];
                }
            }
        }

        waitingForLoginData = true;
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest { CatalogVersion = "Currency" }, GetCurrencyOfferSuccess, GetCurrencyOfferFailure);
        while (waitingForLoginData)
        {
            yield return null;
        }
    }

    void GetCurrencyOfferSuccess(GetCatalogItemsResult getCatalogItemsResult)
    {
        currencyOffers = getCatalogItemsResult;
        waitingForLoginData = false;
    }

    void GetCurrencyOfferFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
