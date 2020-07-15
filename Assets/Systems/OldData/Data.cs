using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;


[System.Serializable]
public class Data : MonoBehaviour
{
    //float logTimer = 0f;

    //long memory;
    //long heap;
    //long usedHeap;
    //long graphics;
    //long used;
    //long allocated;
    //long reserved;
    //long unused;

    public bool initialized = false;

	public string displayName;
    public string playfabID;
    public string entityID;
    public string entityType;
    public TutorialData tutorial;
	public WorldData world;
	public CollectionData collection;
    public AllShopData shop;
	public BattleData battle;
	public BattleData raidBattle;
	public BattleData pvpBattle;
	public CurrencyData currency;
	public GetCatalogItemsResult currencyOffers;
    public GameValues values;
    public AllChestsData chests;
    public EventData pvpEvent;
	public AllRaidData raids;
    public SpecialDealsData deals;
    public List<QuestData> quests;
	public DateTime lastQuestTimeStamp;
    public List<MailData> mail;
    public DateTime lastMailCheck;
    public GuildData guild;

#if UNITY_WEBGL
    public Dictionary<string, int> kongOffers;
#endif

	public static Data instance;

    public bool set = false;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(transform.gameObject);
	}

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        if(playerInfo.PlayerProfile != null)
        {
            displayName = playerInfo.PlayerProfile.DisplayName;
        }

        tutorial.Login(playerInfo);
		world.Login(playerInfo);
        values.Login(playerInfo);
        currency.Login(playerInfo);
        collection.Login(playerInfo);
        chests.Login(playerInfo);
        shop.Login(playerInfo);
		raids.Login(playerInfo);
        deals.Login(playerInfo);
		guild.Login(playerInfo);
        LoginQuests(playerInfo);
		LoginMail(playerInfo);
        LoginPvPEvent(playerInfo);

        playfabID = playerInfo.AccountInfo.PlayFabId;

		battle = new BattleData();
		raidBattle = new BattleData();
		pvpBattle = new BattleData();

#if UNITY_WEBGL
		PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getKongItemList" }, KongItemListSuccess, GetDataFailure);
#endif

		PlayFab.GroupsModels.EntityKey e = new PlayFab.GroupsModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
		PlayFab.GroupsModels.ListMembershipRequest request = new PlayFab.GroupsModels.ListMembershipRequest() { Entity = e };
		PlayFabGroupsAPI.ListMembership(request, MembershipResponse, GetDataFailure);
		PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest { CatalogVersion = "Currency" }, SetCurrencyOfferData, GetDataFailure);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest { CatalogVersion = "Cards" }, SetNotOwnedCards, GetDataFailure);
	}


#if UNITY_WEBGL
void KongItemListSuccess(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			kongOffers = PlayFabSimpleJson.DeserializeObject<Dictionary<string, int>>((string)result.FunctionResult);
		}
	}
#endif

	void MembershipResponse(PlayFab.GroupsModels.ListMembershipResponse response)
	{
		if (response.Groups.Count > 0)
		{
			ChatManager.instance.guildChatRoom = response.Groups[0].GroupName;
		}
	}

    void LoginPvPEvent(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        string eventJson;
        if (playerInfo.TitleData.TryGetValue("PvPEventv2", out eventJson))
        {
            pvpEvent = PlayFabSimpleJson.DeserializeObject<EventData>(eventJson);
        }
    }

    public int CurrentPvPEventTier(int asyncPoints)
    {
        for (int i = 0; i < pvpEvent.Point.Count; i++)
        {
            if (pvpEvent.Point[i].Req > asyncPoints)
            {
                return i;
            }
        }
        return -1;
    }

    //MAIL
    void LoginMail(GetPlayerCombinedInfoResultPayload playerInfo)
	{
		UserDataRecord userDataRecord;

		if (playerInfo.UserReadOnlyData.TryGetValue("Mail", out userDataRecord))
		{
			mail = PlayFabSimpleJson.DeserializeObject<List<MailData>>(userDataRecord.Value);
		}

        lastMailCheck = DateTime.UtcNow;
    }

    void CheckForMail()
    {
        TimeSpan t = DateTime.UtcNow - lastMailCheck;
        if (t.TotalMinutes > 5)
        {
            PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest() { Keys = new List<string>() { "Mail" } }, MailCheckReturned, GetDataFailure);
            lastMailCheck = DateTime.UtcNow;
        }
    }

    void MailCheckReturned(GetUserDataResult result)
    {
        UserDataRecord userDataRecord;
        if (result.Data.TryGetValue("Mail", out userDataRecord))
        {
            List<MailData> newMailList = PlayFabSimpleJson.DeserializeObject<List<MailData>>(userDataRecord.Value);
            if(mail.Count < newMailList.Count)
            {
                mail = newMailList;
                if(NavBar.instance != null)
                {
                    NavBar.instance.SetMailNotification();
                }
                if(Mail.instance != null)
                {
                    Mail.instance.SetMailDisplay();
                }
            }
        }
    }
    //MAIL

    void LoginQuests(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        if (playerInfo.UserReadOnlyData.TryGetValue("Quests", out userDataRecord))
        {
            SetQuestData(userDataRecord.Value);
        }
	}

	public void SetQuestData(string json)
    {
        Dictionary<string, object> dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(json);
        long timeStamp = Convert.ToInt64(dict["Date"]);
        lastQuestTimeStamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timeStamp);
        quests = PlayFabSimpleJson.DeserializeObject<List<QuestData>>(dict["Quests"].ToString());
    }

    public void SetCurrencyOfferData(GetCatalogItemsResult getCatalogItemsResult)
    {
        currencyOffers = getCatalogItemsResult;
    }

    public void SetNotOwnedCards(GetCatalogItemsResult getCatalogItemsResult)
    {
        foreach(CatalogItem item in getCatalogItemsResult.Catalog)
        {
            CardData card = Resources.Load("Cards/" + item.ItemId) as CardData;

            if(card != null)
            {
                if (!Data.instance.collection.inventory.ContainsKey(item.ItemId))
                {
                    card.itemID = item.ItemId;
                    card.displayName = item.DisplayName;
                    card.level = 1;
                    card.starLevel = 0;
                    card.amountOwned = 0;
					
					if(card.type == CardType.Hero)
					{
						collection.heroNotOwned.Add(item.ItemId);
					}
					else if (card.type == CardType.Stronghold)
					{
						collection.strongholdNotOwned.Add(item.ItemId);
					}
					else
					{
						collection.cardNotOwned.Add(item.ItemId);
					}
				}

                collection.allCards.Add(item.ItemId, card);
            }
        }

        set = true;
    }

    public void TutorialFinished(ExecuteCloudScriptResult result)
    {
        Data.instance.tutorial.steps["Finished"] = true;
    }

    public void GetDataFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}

	private void FixedUpdate()
    {
        if(!set)
            return;

        currency.UpdateEnergyRecharge();
        currency.UpdateScrollsRecharge();
        CheckForMail();

		//logTimer += Time.deltaTime;
		//if(logTimer > 5)
		//{
		//	logTimer = 0;
		//	memory = SubmitNumber(System.GC.GetTotalMemory(false), memory);
		//	Debug.Log("Memory: " + FormatNumber(memory));
		//	heap = SubmitNumber(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong(), heap);
		//	Debug.Log("Heap: " + FormatNumber(heap));
		//	usedHeap = SubmitNumber(UnityEngine.Profiling.Profiler.usedHeapSizeLong, usedHeap);
		//	Debug.Log("Unused Heap: " + FormatNumber(usedHeap));
		//	graphics = SubmitNumber(UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver(), graphics);
		//	Debug.Log("Graphics: " + FormatNumber(graphics));
		//	used = SubmitNumber(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(), used);
		//	Debug.Log("Used: " + FormatNumber(used));
		//	allocated = SubmitNumber(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong(), allocated);
		//	Debug.Log("Allocated: " + FormatNumber(allocated));
		//	reserved = SubmitNumber(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong(), reserved);
		//	Debug.Log("Reserved: " + FormatNumber(reserved));
		//}
    }

	//long SubmitNumber(long num, long old)
	//{
	//	if (num > old)
	//	{
	//		old = num;
	//	}
	//	else
	//	{
	//		num = old;
	//	}
	//	return num;
	//}

	//string FormatNumber(long num)
	//{
	//	if (num >= 1000000)
	//		return FormatNumber(num / 1000000) + "M";


	//	return num.ToString("#,0");
	//}
}
