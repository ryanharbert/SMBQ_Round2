using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class EventRewardDisplay : MonoBehaviour
{
    public GameObject displayObject;
    
    public GameObject loading;

    public Text header;

    public NavBarToggle point;
    public NavBarToggle rank;
    public NavBarToggle leaderboard;

    public GameObject pointsObject;
    public GameObject rankObject;
    public GameObject leaderboardObject;

    //POINTS
    public PointTierDisplay[] pointTiers;

    //RANK
    public ChestDisplay[] chestDisplays;

    //LEADERBOARD
    public List<LeaderboardPlayer> topPlayers;
    public LeaderboardPlayer you;
    public GameObject loadingObject;
    public GameObject topPlayersObject;
    public ScrollRect topPlayersScroll;

    public EventData displayEvent;
    public string type;

    public void PvP()
    {
        type = "pvp";
        displayEvent = Data.instance.pvpEvent;
        header.text = "Arena Event";
        SetDisplay();
    }

    public void Raid()
    {
        type = "raid";
        displayEvent = Data.instance.raids.currentEvent;
        header.text = "Raid Event";
        SetDisplay();
    }

    void SetDisplay()
    {
        displayObject.SetActive(true);

		if (point.toggle.isOn)
		{
			PointDisplay();
		}
		else if (rank.toggle.isOn)
		{
			RankDisplay();
		}
		else
		{
			LeaderboardDisplay();
		}
	}

    private void Start()
    {
        point.toggle.isOn = true;
    }

    //TOGGLES
    public void Point(bool on)
    {
        ToggleGameObject(pointsObject, on, point);
        PointDisplay();
    }

    public void Rank(bool on)
    {
        ToggleGameObject(rankObject, on, rank);
        RankDisplay();
    }

    public void Leaderboard(bool on)
    {
        ToggleGameObject(leaderboardObject, on, leaderboard);
        LeaderboardDisplay();
    }

    public void ToggleGameObject(GameObject go, bool active, NavBarToggle toggle)
    {
        if (active)
        {
            go.SetActive(true);
            toggle.toggleText.color = NavBar.instance.highlightColor;
            toggle.toggleText.fontSize = 60;
        }
        else
        {
            go.SetActive(false);
            toggle.toggleText.color = NavBar.instance.normalColor;
            toggle.toggleText.fontSize = 55;
        }
    }
    //TOGGLES

    //POINTS
    public void PointDisplay()
    {
        for(int i = 0; i < pointTiers.Length; i++)
        {
            if (displayEvent.Point.Count > i)
            {
                pointTiers[i].gameObject.SetActive(true);
                int points = Data.instance.currency.raidPoints;
                if (type == "pvp")
                {
                    points = Data.instance.currency.asyncPoints;
                }
                pointTiers[i].Set(i + 1, displayEvent.Point[i], points, type);
            }
            else
            {
                pointTiers[i].gameObject.SetActive(false);
            }
        }
    }
    //POINTS


    //RANK
    public void RankDisplay()
    {
        for(int i = 0; i < chestDisplays.Length; i++)
        {
            if(displayEvent.Rank.Count > i)
            {
                chestDisplays[i].gameObject.SetActive(true);
            }
            else
            {
                chestDisplays[i].gameObject.SetActive(false);
            }
        }

		for(int i = 0; i < displayEvent.Rank.Count; i++)
		{
			int min = displayEvent.Rank[i].Req;
			int max = 1;
			if ((i + 1) < displayEvent.Rank.Count)
			{
				max = displayEvent.Rank[i + 1].Req + 1;
			}
			else
			{
				max = min;
			}

            chestDisplays[displayEvent.Rank.Count - 1 - i].Set(displayEvent.Rank[i].Chest);
            if (min != max)
			{
				chestDisplays[displayEvent.Rank.Count - 1 - i].displayName.text = "Rank " + max + " -" + min;
			}
			else
			{
				chestDisplays[displayEvent.Rank.Count - 1 - i].displayName.text = "Rank " + min;
			}
		}
    }
    //RANK


    //LEADERBOARD
    public void LeaderboardDisplay()
    {
        string statName = "RaidPoints";
        if(type == "pvp")
        {
            statName = "AsyncPoints";
        }
        you.playerName.text = Data.instance.user.displayName;
        you.pvpPoints.text = "???";
        you.rank.text = "???";
        loadingObject.SetActive(true);
        topPlayersObject.SetActive(false);
        topPlayersScroll.verticalNormalizedPosition = 1;
        PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest() { StartPosition = 0, MaxResultsCount = 20, StatisticName = statName }, SetLeaderboard, Data.instance.GetDataFailure);
        PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest() { MaxResultsCount = 1, PlayFabId = Data.instance.user.playfabID, StatisticName = statName }, SetYourLeaderboardPosition, Data.instance.GetDataFailure);
    }

    void SetLeaderboard(GetLeaderboardResult result)
    {
        loadingObject.SetActive(false);
        topPlayersObject.SetActive(true);

        for (int i = 0; i < topPlayers.Count; i++)
        {
            if (result.Leaderboard.Count > i)
            {
                topPlayers[i].gameObject.SetActive(true);
                topPlayers[i].rank.text = (result.Leaderboard[i].Position + 1).ToString();
                topPlayers[i].playerName.text = result.Leaderboard[i].DisplayName;
                topPlayers[i].pvpPoints.text = result.Leaderboard[i].StatValue.ToString();
                topPlayers[i].playfabID = result.Leaderboard[i].PlayFabId;
                if (type == "pvp")
                {
                    topPlayers[i].asyncPointIcon.SetActive(true);
                    topPlayers[i].raidPointIcon.SetActive(false);
                }
                else
                {
                    topPlayers[i].asyncPointIcon.SetActive(false);
                    topPlayers[i].raidPointIcon.SetActive(true);
                }
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
        if (type == "pvp")
        {
            you.asyncPointIcon.SetActive(true);
            you.raidPointIcon.SetActive(false);
        }
        else
        {
            you.asyncPointIcon.SetActive(false);
            you.raidPointIcon.SetActive(true);
        }
    }
    //LEADERBOARD
}
