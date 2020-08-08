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
    public static Data instance;

    public Platforms platform;
    public PlayfabData user;
    public TutorialData tutorial;
	public WorldData world;
	public CollectionData collection;
    public ShopDataManager shop;
	public BattleData battle;
	public BattleData raidBattle;
	public BattleData pvpBattle;
	public CurrencyData currency;
    public GameValues values;
    public AllChestsData chests;
    public EventData pvpEvent;
	public AllRaidData raids;
    public SpecialDealsData deals;
    public QuestDataManager quest;
    public MailDataManager mail;
    public GuildData guild;
    public PvPDataManager pvp;
    public SettingsDataManager settings;

    public bool loginComplete = false;

	private void Awake()
	{
		instance = this;
		DontDestroyOnLoad(transform.gameObject);

        loginComplete = false;
    }

    public void SetLoginData(GetPlayerCombinedInfoResultPayload playerInfo, Action loginSuccess)
    {
        StartCoroutine(SettingData(playerInfo, loginSuccess));
    }

    IEnumerator SettingData(GetPlayerCombinedInfoResultPayload playerInfo, Action loginSuccess)
    {
        tutorial.Login(playerInfo);
		world.Login(playerInfo);
        values.Login(playerInfo);
        currency.Login(playerInfo);
        chests.Login(playerInfo);
		raids.Login(playerInfo);
        deals.Login(playerInfo);
        quest.Login(playerInfo);
		mail.Login(playerInfo);
        pvp.Login(playerInfo);

        battle = new BattleData();
		raidBattle = new BattleData();
		pvpBattle = new BattleData();

        yield return StartCoroutine(collection.Login(playerInfo));
        yield return StartCoroutine(shop.Login(playerInfo));
        yield return StartCoroutine(guild.Login(playerInfo));

        yield return StartCoroutine(ChatManager.instance.Connect());

        loginSuccess.Invoke();
        loginComplete = true;
    }

    public void GetDataFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
	}

	private void FixedUpdate()
    {
        if(!loginComplete)
            return;

        currency.UpdateEnergyRecharge();
        currency.UpdateScrollsRecharge();
        mail.CheckForMail();
    }
}
