using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using Photon.Pun;
using Photon.Realtime;

public class PvP : MonoBehaviourPunCallbacks
{
    public static PvP instance;

    //EVENT REWARDS POP UP
    public EventRewardDisplay eventRewardDisplay;
        
    //LEADERBOARD DISPLAY
    public List<LeaderboardPlayer> topPlayers;
    public LeaderboardPlayer you;
    public GameObject loadingObject;
    public GameObject topPlayersObject;
    public ScrollRect topPlayersScroll;
	public Text eventTimer;

    //EVENT REWARDS
    public GameObject nextRewardObject;
    public Text nextRewardText;
    public Text rankText;

    //ARENA OPPONENT SELECTION
    public GameObject opponentSelectObject;
	public GameObject opponentLoadingObject;
	public PvPOpponent[] opponents;

	//SCROLL ENERGY SWITCH
    public GameObject energyCurrency;
    public GameObject scrollsCurrency;

    //PLAYER PROFILE
    public GameObject profileLoadingObject;
    public GameObject playerProfileBackgroundObject;
    public PvPOpponent profileDisplayOpponent;
    public LeaderboardPlayer playerToDisplay;

    private void Awake()
    {
        instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        energyCurrency.SetActive(false);
        scrollsCurrency.SetActive(true);
        SetPvPDisplay();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        energyCurrency.SetActive(true);
        scrollsCurrency.SetActive(false);
    }

    public void Update()
    {
        DateTime d = DateTime.UtcNow;
        int dayOfWeek = (int)d.DayOfWeek;
        if (dayOfWeek == 0)
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

    //PVP DISPLAY
    public void SetPvPDisplay()
    {
        if (Data.instance.pvp.CurrentPvPEventTier(Mathf.RoundToInt(Data.instance.currency.asyncPoints)) == -1)
        {
            rankText.gameObject.SetActive(true);
            rankText.text = "Rank ???";
            nextRewardObject.SetActive(false);
        }
        else
        {
            rankText.gameObject.SetActive(false);
            nextRewardObject.SetActive(true);
            nextRewardText.text = Data.instance.currency.asyncPoints + " / " + Data.instance.pvpEvent.Point[Data.instance.pvp.CurrentPvPEventTier(Data.instance.currency.asyncPoints)].Req;
        }

        you.playerName.text = Data.instance.user.displayName;
        you.pvpPoints.text = "???";
        you.rank.text = "???";
        loadingObject.SetActive(true);
        topPlayersObject.SetActive(false);
        topPlayersScroll.verticalNormalizedPosition = 1;
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest() { StartPosition = 0, MaxResultsCount = 20, StatisticName = "AsyncPoints" }, SetLeaderboard, PvPServerFailure);
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest() { MaxResultsCount = 1, PlayFabId = Data.instance.user.playfabID, StatisticName = "AsyncPoints" }, SetYourLeaderboardPosition, PvPServerFailure);
    }

    void SetLeaderboard(GetLeaderboardResult result)
    {
        loadingObject.SetActive(false);
        topPlayersObject.SetActive(true);
        
        for(int i = 0; i < topPlayers.Count; i++)
        {
            if(result.Leaderboard.Count > i)
            {
                topPlayers[i].gameObject.SetActive(true);
                topPlayers[i].rank.text = (result.Leaderboard[i].Position + 1).ToString();
                topPlayers[i].playerName.text = result.Leaderboard[i].DisplayName;
                topPlayers[i].pvpPoints.text = result.Leaderboard[i].StatValue.ToString();
                topPlayers[i].playfabID = result.Leaderboard[i].PlayFabId;
            }
            else
            {
                topPlayers[i].gameObject.SetActive(false);
            }
        }
    }

    void SetYourLeaderboardPosition(GetLeaderboardAroundPlayerResult result)
    {
        you.rank.text = (result.Leaderboard[0].Position + 1).ToString();
        you.pvpPoints.text = result.Leaderboard[0].StatValue.ToString();

        rankText.text = "Rank #" + (result.Leaderboard[0].Position + 1);
    }
    //PVP DISPLAY


    //EVENT REWARDS POP UP
    public void OpenPvPEventRewards()
    {
        eventRewardDisplay.PvP();
    }
    //EVENT REWARDS POP UP


    //ASYNC PVP
    public void ArenaFightButton()
    {
        SelectOpponent();
    }

    void SelectOpponent()
	{
        if(Data.instance.currency.scrolls > 0)
        {
            opponentSelectObject.SetActive(true);
            opponentLoadingObject.SetActive(true);
            foreach (PvPOpponent o in opponents)
            {
                o.gameObject.SetActive(false);
            }
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "startPvPFight" }, OpponentsReturned, PvPServerFailure);
            Data.instance.currency.scrolls -= 1;
        }
        else
        {
            Warning.instance.Activate("You need to wait for more Scrolls to fight");
        }
	}

	public void OpponentsReturned(ExecuteCloudScriptResult result)
	{
		if (result.FunctionResult != null)
		{
			Dictionary<string, object> dict = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(result.FunctionResult.ToString());
			int i = 0;

			foreach(KeyValuePair<string, object> o in dict)
			{
				if ((dict.Count - 1) >= i && i < 3)
				{
					opponents[i].gameObject.SetActive(true);
					opponents[i].nameText.text = o.Key;
					Dictionary<string, object> opponentData = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(o.Value.ToString());
					opponents[i].rankText.text = "Rank " + (Convert.ToInt32(opponentData["Rank"]) + 1);
					opponents[i].pointsText.text = opponentData["Points"].ToString();
					Dictionary<string, object> opponentDeck = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(opponentData["Deck"].ToString());
					opponents[i].deck = PlayFabSimpleJson.DeserializeObject<DeckData>(opponentDeck["Names"].ToString());
					opponents[i].levels = PlayFabSimpleJson.DeserializeObject<DeckLevelsData>(opponentDeck["Levels"].ToString());
                    if(opponentDeck.ContainsKey("Stars"))
                    {
                        opponents[i].stars = PlayFabSimpleJson.DeserializeObject<DeckStarsData>(opponentDeck["Stars"].ToString());
                    }
                    else
                    {
                        opponents[i].stars = new DeckStarsData();
                        opponents[i].stars.deck = new int[8];
                        for (int h = 0; h < opponents[i].stars.deck.Length; h++)
                        {
                            opponents[i].stars.deck[h] = 0;
                        }
                        opponents[i].stars.stronghold = 0;
                        opponents[i].stars.hero = 0;
                    }
                    opponents[i].strongholdDisplay.SetCardDisplay(opponents[i].deck.stronghold);
                    opponents[i].strongholdLevelText.text = opponents[i].levels.stronghold.ToString();
                    SetStarDisplay(opponents[i].stars.stronghold, opponents[i].strongholdDisplay);
                    opponents[i].heroDisplay.SetCardDisplay(opponents[i].deck.heroes[0]);
					opponents[i].heroLevelText.text = opponents[i].levels.hero.ToString();
                    SetStarDisplay(opponents[i].stars.hero, opponents[i].heroDisplay);
                    for (int h = 0; h < opponents[i].deckDisplay.Length; h++)
					{
						opponents[i].deckDisplay[h].SetCardDisplay(opponents[i].deck.deck[h]);
                        SetStarDisplay(opponents[i].stars.deck[h], opponents[i].deckDisplay[h]);
                    }
					for (int h = 0; h < opponents[i].deckLevelsText.Length; h++)
					{
						opponents[i].deckLevelsText[h].text = opponents[i].levels.deck[h].ToString();
					}
				}
				i++;
			}

            while(i < 3)
            {
                opponents[i].gameObject.SetActive(true);
                opponents[i].deck = new DeckData();
                opponents[i].deck.deck = new string[8];
                opponents[i].deck.deck[0] = "RedDragon";
                opponents[i].deck.deck[1] = "Guards";
                opponents[i].deck.deck[2] = "RockGolem";
                opponents[i].deck.deck[3] = "Crows";
                opponents[i].deck.deck[4] = "SkeletonArchers";
                opponents[i].deck.deck[5] = "SkeletonSorcerer";
                opponents[i].deck.deck[6] = "ElvenArchers";
                opponents[i].deck.deck[7] = "Barbearian";
                opponents[i].levels = new DeckLevelsData();
                opponents[i].levels.deck = new int[8];
                for (int h = 0; h < opponents[i].levels.deck.Length; h++)
                {
                    opponents[i].levels.deck[h] = UnityEngine.Random.Range(1,4);
                }
                opponents[i].levels.stronghold = UnityEngine.Random.Range(1, 4);
                opponents[i].levels.hero = UnityEngine.Random.Range(1, 4);
                opponents[i].stars = new DeckStarsData();
                opponents[i].stars.deck = new int[8];
                for (int h = 0; h < opponents[i].stars.deck.Length; h++)
                {
                    opponents[i].stars.deck[h] = 0;
                }
                opponents[i].stars.stronghold = 0;
                opponents[i].stars.hero = 0;
                if (i == 0)
                {
                    opponents[i].nameText.text = "Bob5618";
                    opponents[i].rankText.text = "Rank 1";
                    opponents[i].pointsText.text = "10";
                    opponents[i].deck.stronghold = "TreeFort";
                    opponents[i].deck.heroes[0] = "Slime";
                    opponents[i].strongholdDisplay.SetCardDisplay(opponents[i].deck.stronghold);
                    opponents[i].strongholdLevelText.text = opponents[i].levels.stronghold.ToString();
                    SetStarDisplay(opponents[i].stars.stronghold, opponents[i].strongholdDisplay);
                    opponents[i].heroDisplay.SetCardDisplay(opponents[i].deck.heroes[0]);
                    opponents[i].heroLevelText.text = opponents[i].levels.hero.ToString();
                    SetStarDisplay(opponents[i].stars.hero, opponents[i].heroDisplay);
                    for (int h = 0; h < opponents[i].deckDisplay.Length; h++)
                    {
                        opponents[i].deckDisplay[h].SetCardDisplay(opponents[i].deck.deck[h]);
                        SetStarDisplay(opponents[i].stars.deck[h], opponents[i].deckDisplay[h]);
                    }
                    for (int h = 0; h < opponents[i].deckLevelsText.Length; h++)
                    {
                        opponents[i].deckLevelsText[h].text = opponents[i].levels.deck[h].ToString();
                    }
                }
                else if(i == 1)
                {
                    opponents[i].nameText.text = "Ted382";
                    opponents[i].rankText.text = "Rank 2";
                    opponents[i].pointsText.text = "2";
                    opponents[i].deck.stronghold = "CannonBase";
                    opponents[i].deck.heroes[0] = "Mimic";
                    opponents[i].strongholdDisplay.SetCardDisplay(opponents[i].deck.stronghold);
                    opponents[i].strongholdLevelText.text = opponents[i].levels.stronghold.ToString();
                    SetStarDisplay(opponents[i].stars.stronghold, opponents[i].strongholdDisplay);
                    opponents[i].heroDisplay.SetCardDisplay(opponents[i].deck.heroes[0]);
                    opponents[i].heroLevelText.text = opponents[i].levels.hero.ToString();
                    SetStarDisplay(opponents[i].stars.hero, opponents[i].heroDisplay);
                    for (int h = 0; h < opponents[i].deckDisplay.Length; h++)
                    {
                        opponents[i].deckDisplay[h].SetCardDisplay(opponents[i].deck.deck[h]);
                        SetStarDisplay(opponents[i].stars.deck[h], opponents[i].deckDisplay[h]);
                    }
                    for (int h = 0; h < opponents[i].deckLevelsText.Length; h++)
                    {
                        opponents[i].deckLevelsText[h].text = opponents[i].levels.deck[h].ToString();
                    }
                }
                else if(i == 2)
                {
                    opponents[i].nameText.text = "AwesomeGuy23";
                    opponents[i].rankText.text = "Rank 3";
                    opponents[i].pointsText.text = "1";
                    opponents[i].deck.stronghold = "DoomCastle";
                    opponents[i].deck.heroes[0] = "Reaper";
                    opponents[i].strongholdDisplay.SetCardDisplay(opponents[i].deck.stronghold);
                    opponents[i].strongholdLevelText.text = opponents[i].levels.stronghold.ToString();
                    SetStarDisplay(opponents[i].stars.stronghold, opponents[i].strongholdDisplay);
                    opponents[i].heroDisplay.SetCardDisplay(opponents[i].deck.heroes[0]);
                    opponents[i].heroLevelText.text = opponents[i].levels.hero.ToString();
                    SetStarDisplay(opponents[i].stars.hero, opponents[i].heroDisplay);
                    for (int h = 0; h < opponents[i].deckDisplay.Length; h++)
                    {
                        opponents[i].deckDisplay[h].SetCardDisplay(opponents[i].deck.deck[h]);
                        SetStarDisplay(opponents[i].stars.deck[h], opponents[i].deckDisplay[h]);
                    }
                    for (int h = 0; h < opponents[i].deckLevelsText.Length; h++)
                    {
                        opponents[i].deckLevelsText[h].text = opponents[i].levels.deck[h].ToString();
                    }
                }

                i++;
            }

			opponentLoadingObject.SetActive(false);
		}
	}

    void SetStarDisplay(int starLevel, CardDisplay c)
    {
        switch (starLevel)
        {
            case 0:
                c.starFrame.SetActive(false);
                break;
            case 1:
                c.starFrame.SetActive(true);
                c.star1.SetActive(true);
                c.star2.SetActive(false);
                c.star3.SetActive(false);
                break;
            case 2:
                c.starFrame.SetActive(true);
                c.star1.SetActive(true);
                c.star2.SetActive(true);
                c.star3.SetActive(false);
                break;
            case 3:
                c.starFrame.SetActive(true);
                c.star1.SetActive(true);
                c.star2.SetActive(true);
                c.star3.SetActive(true);
                break;
        }
    }
    //ASYNC PVP


    //LIVE PVP
    public void LivePvPFightButton()
    {
        EnterMatchmaking();
    }

    public void EnterMatchmaking()
	{
        PhotonNetwork.OfflineMode = false;

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
		PhotonNetwork.LocalPlayer.NickName = Data.instance.user.displayName;

        PunManager.instance.JoinQue("Finding your opponent...");
    }

	public override void OnConnectedToMaster()
	{
		JoinPvPRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("Join Failed: " + message);
		CreatePvPRoom();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Creating Failed: " + message);
		JoinPvPRoom();
	}

	private void CreatePvPRoom()
    {
        Debug.Log("Creating Room");
        RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsVisible = true;
		roomOptions.MaxPlayers = 2;
		roomOptions.CleanupCacheOnLeave = false;

		roomOptions.CustomRoomPropertiesForLobby = new string[1] { "type" };
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "type", "pvp" } };

		PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
	}

	private void JoinPvPRoom()
    {
        Debug.Log("Joining Room");
        ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable() { { "type", "pvp" } };

		PhotonNetwork.JoinRandomRoom(h, 2);
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("In Room");
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
		if(PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
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
		Debug.Log("Start Game");
		PhotonNetwork.CurrentRoom.IsVisible = false;
		PhotonNetwork.CurrentRoom.IsOpen = false;
		SceneLoader.LiveBattle(Data.instance.pvpBattle, BattleType.LivePvP, Data.instance.pvpBattle.battleScene);
	}
	//LIVE PVP


    //PLAYERPROFILE
    public void GetPlayerInfo(LeaderboardPlayer player)
    {
        playerToDisplay = player;
        profileLoadingObject.SetActive(true);
        playerProfileBackgroundObject.SetActive(true);
        profileDisplayOpponent.gameObject.SetActive(false);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getPlayerDeck", FunctionParameter = new { playFabId = player.playfabID } }, PlayerInfoReceived, PvPServerFailure);
    }

    public void PlayerInfoReceived(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            profileLoadingObject.SetActive(false);
            playerProfileBackgroundObject.SetActive(true);
            profileDisplayOpponent.gameObject.SetActive(true);
            
            profileDisplayOpponent.nameText.text = playerToDisplay.playerName.text;
            profileDisplayOpponent.rankText.text = "Rank " + playerToDisplay.rank.text;
            profileDisplayOpponent.pointsText.text = playerToDisplay.pvpPoints.text;
            Dictionary<string, object> opponentDeck = PlayFabSimpleJson.DeserializeObject<Dictionary<string, object>>(result.FunctionResult.ToString());
            profileDisplayOpponent.deck = PlayFabSimpleJson.DeserializeObject<DeckData>(opponentDeck["Names"].ToString());
            profileDisplayOpponent.levels = PlayFabSimpleJson.DeserializeObject<DeckLevelsData>(opponentDeck["Levels"].ToString());
            if (opponentDeck.ContainsKey("Stars"))
            {
                profileDisplayOpponent.stars = PlayFabSimpleJson.DeserializeObject<DeckStarsData>(opponentDeck["Stars"].ToString());
            }
            else
            {
                profileDisplayOpponent.stars = new DeckStarsData();
                profileDisplayOpponent.stars.deck = new int[8];
                for (int h = 0; h < profileDisplayOpponent.stars.deck.Length; h++)
                {
                    profileDisplayOpponent.stars.deck[h] = 0;
                }
                profileDisplayOpponent.stars.stronghold = 0;
                profileDisplayOpponent.stars.hero = 0;
            }
            for (int h = 0; h < profileDisplayOpponent.deckDisplay.Length; h++)
            {
                profileDisplayOpponent.deckDisplay[h].SetCardDisplay(profileDisplayOpponent.deck.deck[h]);
                SetStarDisplay(profileDisplayOpponent.stars.deck[h], profileDisplayOpponent.deckDisplay[h]);
            }
            for (int h = 0; h < profileDisplayOpponent.deckLevelsText.Length; h++)
            {
                profileDisplayOpponent.deckLevelsText[h].text = profileDisplayOpponent.levels.deck[h].ToString();
            }
            profileDisplayOpponent.strongholdDisplay.SetCardDisplay(profileDisplayOpponent.deck.stronghold);
            SetStarDisplay(profileDisplayOpponent.stars.stronghold, profileDisplayOpponent.strongholdDisplay);
            profileDisplayOpponent.strongholdLevelText.text = profileDisplayOpponent.levels.stronghold.ToString();
            profileDisplayOpponent.heroDisplay.SetCardDisplay(profileDisplayOpponent.deck.heroes[0]);
            SetStarDisplay(profileDisplayOpponent.stars.hero, profileDisplayOpponent.heroDisplay);
            profileDisplayOpponent.heroLevelText.text = profileDisplayOpponent.levels.hero.ToString();
        }
    }
    //PLAYERPROFILE


    private void PvPServerFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
