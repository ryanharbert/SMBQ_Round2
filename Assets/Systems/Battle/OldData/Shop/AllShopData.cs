using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

[System.Serializable]
public class AllShopData
{
	public int month;
	public int day;
    public Dictionary<string, ShopData> shops;
    public string currentShop;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
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
    }
}
