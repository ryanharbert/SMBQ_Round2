using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class WorldShop : MonoBehaviour
{
	public UIButton exit;

    public Text shopResetTimer;

	public RandomOffer[] randomOffer;
	public OfferConfirmation randomOfferConfirmation;

    public GameObject loading;

	public Chest chest;
	public ChestConfirmation chestConfirmation;

    public static WorldShop instance;

	private void Awake()
	{
        //float defaultWidth = Camera.main.orthographicSize * (4f / 3f);
        //Camera.main.orthographicSize = defaultWidth / Camera.main.aspect;

        instance = this;

		exit.buttonFunction = LeaveShop;
	}

	private void Start()
	{
		chest.SetChest();
        SetShop();
    }

    void SetShop()
    {
        if (Data.instance.shop.shops == null || Data.instance.shop.month != DateTime.UtcNow.Month || Data.instance.shop.day != DateTime.UtcNow.Day)
        {
            Data.instance.shop.month = DateTime.UtcNow.Month;
            Data.instance.shop.day = DateTime.UtcNow.Day;
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "resetUserShop" }, ShopDataReturned, Data.instance.GetDataFailure);

            for (int i = 0; i < randomOffer.Length; i++)
            {
                randomOffer[i].gameObject.SetActive(false);
            }
            loading.SetActive(true);
        }
        else
        {
            for (int i = 0; i < randomOffer.Length; i++)
            {
                randomOffer[i].gameObject.SetActive(true);
                randomOffer[i].SetRandomOffer(i, true);
            }
        }
    }

    public void ShopDataReturned(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            Data.instance.shop.shops = PlayFabSimpleJson.DeserializeObject<Dictionary<string, ShopData>>(result.FunctionResult.ToString());

            foreach (KeyValuePair<string, ShopData> s in Data.instance.shop.shops)
            {
                s.Value.Purchased = new List<bool>();
                for (int i = 0; i < s.Value.Contents.Count; i++)
                {
                    s.Value.Purchased.Add(false);
                }
            }
            loading.SetActive(false);
            SetShop();
        }
        else
        {
            Debug.LogError("Offer does not exist.");
        }
    }

    public void LeaveShop()
	{
		SceneLoader.ChangeScenes("WorldMap");
    }

    private void Update()
    {
        shopResetTimer.text = ResetTime.Get();
    }
}
