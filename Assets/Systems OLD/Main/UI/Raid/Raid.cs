using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Photon.Pun;
using Photon.Realtime;

public class Raid : MonoBehaviourPunCallbacks
{

    public static Raid instance;

    //EVENT REWARDS POP UP
    public EventRewardDisplay eventRewardDisplay;

    //Entry
    public GameObject raidEntryOptions;
    public QueType queType = QueType.RandomRaid;

    //Locked
	public GameObject lockedRaid;
    public Text levelReqText;
	public Button fightButton;
	public Image buttonImage;

    //Raid Display
    public Image raidImage;
    public Text raidName;
    public Text raidDifficulty;
    public Text currentRaid;

    //Event Rewards
    public GameObject nextRewardObject;
    public Text nextRewardText;
    public Text rankText;
    public Text eventTimer;

    //Chest Slots
    public ChestSlotDisplay[] chestSlots;


	private void Awake()
    {
        instance = this;
    }

	public override void OnEnable()
	{
		base.OnEnable();

		SetRaidDisplay();
        SetChestSlots();
        EventDisplay();
    }

    private void Update()
    {
        DateTime d = DateTime.UtcNow;
        int dayOfWeek = (int)d.DayOfWeek;
        if(dayOfWeek == 0)
        {
            dayOfWeek = 1;
        }
        else
        {
            dayOfWeek = 8 - dayOfWeek;
        }
        DateTime n = new DateTime(d.Year, d.Month, d.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(dayOfWeek);

        TimeSpan t = n - d;
        eventTimer.text = "Event ends: " + TimeSpanDisplay.Format(t);
    }

    //EVENT DISPLAY
    public void EventDisplay()
    {
        if (Data.instance.raids.CurrentEventTier(Mathf.RoundToInt(Data.instance.currency.raidPoints)) == -1)
        {
            rankText.gameObject.SetActive(true);
            rankText.text = "Rank ???";
            nextRewardObject.SetActive(false);
            PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest() { MaxResultsCount = 1, PlayFabId = Data.instance.user.playfabID, StatisticName = "RaidPoints" }, SetYourLeaderboardPosition, Data.instance.GetDataFailure);
        }
        else
        {
            rankText.gameObject.SetActive(false);
            nextRewardObject.SetActive(true);
            nextRewardText.text = Data.instance.currency.raidPoints + " / " + Data.instance.raids.currentEvent.Point[Data.instance.raids.CurrentEventTier(Data.instance.currency.raidPoints)].Req;
        }
    }

    void SetYourLeaderboardPosition(GetLeaderboardAroundPlayerResult result)
    {
        rankText.text = "Rank #" + (result.Leaderboard[0].Position + 1);
    }

    public void OpenRaidEventRewards()
    {
        eventRewardDisplay.Raid();
    }
    //EVENT DISPLAY

    //RAID DISPLAY
    public void SetRaidDisplay()
    {
        raidName.text = Data.instance.raids.CurrentRaidName();
        raidImage.sprite = Resources.Load<Sprite>("Raids/" + Data.instance.raids.CurrentRaidName());
        string islandReq = Data.instance.raids.CurrentRaidTemplate().IslandReq;
        if (!Data.instance.world.VisitedIslands.Contains(islandReq))
        {
            lockedRaid.SetActive(true);
            fightButton.interactable = false;
            buttonImage.color = new Color(1, 1, 1, 0.5f);
            levelReqText.text = "Until You Reach " + Data.instance.world.GetIslandName(islandReq);
        }
        else
        {
            lockedRaid.SetActive(false);
            fightButton.interactable = true;
            buttonImage.color = Color.white;
        }

        currentRaid.text = (Data.instance.raids.raidIndex + 1) + " / " + Data.instance.raids.eventRaids.Count;
        raidDifficulty.text = "Level " + Data.instance.raids.currentLevel;
    }

    public void NextRaid()
    {
        Data.instance.raids.ChangeIndex(1);
        SetRaidDisplay();
    }

    public void PreviousRaid()
    {
        Data.instance.raids.ChangeIndex(-1);
        SetRaidDisplay();
    }

    public void NextDifficulty()
    {
        Data.instance.raids.ChangeDifficulty(1);
        SetRaidDisplay();
    }

    public void PreviousDifficulty()
    {
        Data.instance.raids.ChangeDifficulty(-1);
        SetRaidDisplay();
    }
    public void ShowRewards()
	{
		ChestContentsDisplay.instance.Raid(Data.instance.raids.CurrentRaidChest(), Data.instance.raids.currentLevel);
	}
    //RAID DISPLAY

    //ENTER RAID
    public void FightButton()
    {
        Data.instance.raidBattle.enemyName = Data.instance.raids.CurrentRaidName();
        Data.instance.raidBattle.level = Data.instance.raids.currentLevel;
        RaidTemplateData r = Data.instance.raids.CurrentRaidTemplate();
        Data.instance.raidBattle.battleScene = r.BattleScene;
        Data.instance.raidBattle.enemyObjectives = r.Objectives;
        Data.instance.raidBattle.enemyDeck = r.Deck;

        raidEntryOptions.SetActive(true);
    }

    public void SoloRaid()
    {
        raidEntryOptions.SetActive(false);
        SceneLoader.OfflineBattle(Data.instance.raidBattle, BattleType.Raid, Data.instance.raidBattle.battleScene);
    }

    public void RandomRaid()
    {
        raidEntryOptions.SetActive(false);
        queType = QueType.RandomRaid;
        EnterMatchmaking();
    }

    public void GuildRaid()
    {
        raidEntryOptions.SetActive(false);
        queType = QueType.GuildRaid;
        EnterMatchmaking();
    }
    //ENTER RAID

    //CHEST SLOTS
    public void SetChestSlots()
    {
        foreach (ChestSlotDisplay c in chestSlots)
        {
            c.Set(Data.instance.raids.chestSlots[c.slotIndex]);
        }
    }
    //CHEST SLOTS

	//LIVE RAID
	public void EnterMatchmaking()
	{
		PhotonNetwork.OfflineMode = false;

		ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		PhotonNetwork.LocalPlayer.NickName = Data.instance.user.displayName;

        PunManager.instance.JoinQue("Finding your ally...");

    }

	public override void OnConnectedToMaster()
	{
		JoinRaidRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("Join Failed: " + message);
		CreateRaidRoom();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Creating Failed: " + message);
        JoinRaidRoom();
	}

	private void CreateRaidRoom()
	{
		Debug.Log("Joining Room");
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsVisible = true;
		roomOptions.MaxPlayers = 2;
		roomOptions.CleanupCacheOnLeave = false;

		string roomName = Data.instance.raids.CurrentRaidName() + Data.instance.raids.currentLevel;
		if (queType == QueType.GuildRaid)
		{
			roomName += Data.instance.guild.name;
		}
		roomOptions.CustomRoomPropertiesForLobby = new string[1] { "type" };
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", roomName } };

		PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
	}

	private void JoinRaidRoom()
	{
        string roomName = Data.instance.raids.CurrentRaidName() + Data.instance.raids.currentLevel;
        if(queType == QueType.GuildRaid)
        {
            roomName += Data.instance.guild.name;
		}
		ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable() { { "type", roomName } };

		PhotonNetwork.JoinRandomRoom(h, 2);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("In Room");
		Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
		Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
		if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
		{
			StartGame();
		}
	}
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log("Game Starting");
		if (PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
		{
			StartGame();
		}
	}

	private void StartGame()
	{
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.IsOpen = false;
        SceneLoader.LiveBattle(Data.instance.raidBattle, BattleType.LiveRaid, Data.instance.raidBattle.battleScene);
    }
	//LIVE Raid
}
