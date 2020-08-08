using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardPlayer : MonoBehaviour
{
    public Text rank;
    public Text playerName;
    public Text pvpPoints;
	public GameObject raidPointIcon;
	public GameObject asyncPointIcon;
    public string playfabID;

    public void SeePlayerDeck()
    {
        PvP.instance.GetPlayerInfo(this);
    }
}
