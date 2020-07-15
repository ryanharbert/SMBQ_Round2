using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Photon.Pun;


public class GameSystem : BaseSystem
{
	public override void Init(BattleState s)
    {
		s.gameOver = false;
		s.autoPlay = false;
		s.unitRotation = GameObject.FindWithTag("UnitRotation").transform;

		if (Battle.instance.battleType == BattleType.LivePvP)
		{
			LivePvPSetup(s);
		}
		else if(Battle.instance.battleType == BattleType.LiveRaid)
		{
			LiveRaidSetup(s);
		}
		else
		{
			DefaultSetup(s);
		}
    }

    private void DefaultSetup(BattleState s)
    {
        Unit u = PhotonNetwork.Instantiate("Units/" + s.playerStronghold.starLevel + "/" + s.playerStronghold.Unit.name, s.battleLayout.playerStrongholdLocations[0], s.unitRotation.rotation * Quaternion.Euler(0f, -90f, 0f)).GetComponent<Unit>();
        u.photonView.RPC("SetRPC", RpcTarget.All, s.teamID, s.playerStronghold.level);
        s.playerObjectives.Add(u);
        if(Battle.instance.battleType == BattleType.AsyncPvP)
        {
            u.PlayerNameAboveStronghold(Data.instance.displayName);
        }
		s.yourObjective = u;
	}

	private void LiveRaidSetup(BattleState s)
	{
		if (PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			s.teamID = 0;
			Unit u = PhotonNetwork.Instantiate("Units/" + s.playerStronghold.starLevel + "/" + s.playerStronghold.Unit.name, s.battleLayout.playerStrongholdLocations[1], s.unitRotation.rotation * Quaternion.Euler(0f, -90f, 0f)).GetComponent<Unit>();
			u.photonView.RPC("SetRPC", RpcTarget.AllBuffered, s.teamID, s.playerStronghold.level);
			s.playerObjectives.Add(u);
			u.photonView.RPC("FriendlyStronghold", RpcTarget.OthersBuffered);
			u.PlayerNameAboveStronghold(PhotonNetwork.LocalPlayer.NickName);
			s.yourObjective = u;
		}
		else
		{
			s.teamID = 0;
			Unit u = PhotonNetwork.Instantiate("Units/" + s.playerStronghold.starLevel + "/" + s.playerStronghold.Unit.name, s.battleLayout.playerStrongholdLocations[2], s.unitRotation.rotation * Quaternion.Euler(0f, -90f, 0f)).GetComponent<Unit>();
			u.photonView.RPC("SetRPC", RpcTarget.AllBuffered, s.teamID, s.playerStronghold.level);
			s.playerObjectives.Add(u);
			u.photonView.RPC("FriendlyStronghold", RpcTarget.OthersBuffered);
			u.PlayerNameAboveStronghold(PhotonNetwork.LocalPlayer.NickName);
			s.yourObjective = u;
		}
	}

	private void LivePvPSetup(BattleState s)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            s.teamID = 0;
            Unit u = PhotonNetwork.Instantiate("Units/" + s.playerStronghold.starLevel + "/" + s.playerStronghold.Unit.name, s.battleLayout.playerStrongholdLocations[0], s.unitRotation.rotation * Quaternion.Euler(0f, -90f, 0f)).GetComponent<Unit>();
            u.photonView.RPC("SetRPC", RpcTarget.AllBuffered, s.teamID, 1);
            s.playerObjectives.Add(u);
            u.photonView.RPC("EnemyStronghold", RpcTarget.OthersBuffered);
            u.PlayerNameAboveStronghold(PhotonNetwork.LocalPlayer.NickName);
			s.yourObjective = u;
		}
        else
        {
            s.teamID = 1;
            Unit u = PhotonNetwork.Instantiate("Units/" + s.playerStronghold.starLevel + "/" + s.playerStronghold.Unit.name, s.battleLayout.enemyStrongholdLocation, s.unitRotation.rotation * Quaternion.Euler(0f, 180f, 0f)).GetComponent<Unit>();
            u.photonView.RPC("SetRPC", RpcTarget.AllBuffered, s.teamID, 1);
            s.playerObjectives.Add(u);
            u.photonView.RPC("EnemyStronghold", RpcTarget.OthersBuffered);
            u.PlayerNameAboveStronghold(PhotonNetwork.LocalPlayer.NickName);
			s.yourObjective = u;
		}
    }

    public override void Run(BattleState s)
	{
		if(s.gameOver)
			return;

		if(s.playerObjectives.Count == 0 || s.enemyObjectives.Count == 0)
		{
			GameOver(s);
		}

		if(s.autoPlay && s.playerDeck[0].ManaCost <= s.currentMana)
		{
			//if(BattleCardGroup.selectedCard != null)
			//{
			//	BattleCardGroup.selectedCard.Toggle(false);
			//	BattleCardGroup.selectedCard = null;
			//}
			s.cardSelected = 0;
			s.spawnPosition = s.battleLayout.playerAutoSpawnPositions[Random.Range(0, s.battleLayout.playerAutoSpawnPositions.Length)];
			s.currentMana -= s.playerDeck[s.cardSelected].ManaCost;
			s.playerDeck[s.cardSelected].PlayCard(s, s.spawnPosition, s.teamID, s.playerDeck[s.cardSelected].level);

			SendCardToBack(s);
			if (BattleCardGroup.selectedCard != null)
			{
				s.cardSelected = BattleCardGroup.selectedCard.index;
			}
		}
		else if(s.playCard)
		{
			s.playCard = false;
			if (s.cardSelected < 3 && s.cardSelected > -1)
			{
				if (s.playerDeck[s.cardSelected].ManaCost <= s.currentMana)
				{
					s.currentMana -= s.playerDeck[s.cardSelected].ManaCost;
                    if(Battle.instance.battleType != BattleType.LivePvP)
                    {
                        s.playerDeck[s.cardSelected].PlayCard(s, s.spawnPosition, s.teamID, s.playerDeck[s.cardSelected].level);
                    }
                    else
                    {
                        int star = 0;
                        if (s.playerDeck[s.cardSelected].starPowers.Length > 3)
                        {
                            star = 3;
                        }
                        else if (s.playerDeck[s.cardSelected].starPowers.Length > 2)
                        {
                            star = 2;
                        }
                        else if (s.playerDeck[s.cardSelected].starPowers.Length > 1)
                        {
                            star = 1;
                        }
                        s.playerDeck[s.cardSelected].PlayCard(s, s.spawnPosition, s.teamID, 1, star);
                    }

                    SendCardToBack(s);
				}
				else
				{
					BattleWarning.Display(BattleWarningType.NotEnoughMana);
				}
			}
			else if (s.cardSelected >= 4 && s.cardSelected <= 6)
			{
				HeroPlay(s);
			}
			else
			{
				BattleWarning.Display(BattleWarningType.NoCardSelected);
			}
		}
	}

	private void GameOver(BattleState s)
	{
		Time.timeScale = 1;

		if(s.playerObjectives.Count != 0)
		{
			Battle.instance.Win();
		}
		else
		{
			Battle.instance.Lose();
		}

		s.gameOver = true;
    }

    private void SendCardToBack(BattleState s)
    {
        CardData c = s.playerDeck[s.cardSelected];

        for (var i = 3; i < s.playerDeck.Count; i++)
        {
            if (i == 3)
            {
                s.playerDeck[s.cardSelected] = s.playerDeck[i];
				if(s.autoPlay && s.cardSelected == 0)
				{
					Battle.instance.battleCardGroup.battleCards[0].cardDisplay.SetCardDisplay(s.playerDeck[s.cardSelected].itemID);
					Battle.instance.battleCardGroup.battleCards[0].cardData = s.playerDeck[s.cardSelected];
				}
				else
				{
					BattleCardGroup.CardPlayed(s);
				}
            }
            else
            {
                s.playerDeck[i - 1] = s.playerDeck[i];
            }
        }

        s.playerDeck[s.playerDeck.Count - 1] = c;
    }

	void HeroPlay(BattleState s)
	{
		if (s.hero != null)
		{
			if (s.cardSelected == 5)
            {
                if (s.hero.state < 4)
                {
                    if (s.hero.abilities[0].CDTimer <= 0 && !s.hero.abilities[0].disabled)
					{
						s.abilityBeingCast = 1;
						s.hero.abilities[0].CDTimer = s.hero.abilities[0].cooldown;
						s.hero.photonView.RPC("Cast", RpcTarget.MasterClient, s.spawnPosition, 0);
                        BattleCardGroup.CardPlayed(s);
					}
					else
					{
						BattleWarning.Display(BattleWarningType.AbilityOnCooldown);
                    }
                }
                else
                {
	                BattleWarning.Display(BattleWarningType.HeroAlreadyCasting);
                }
            }
			else if (s.cardSelected == 6)
            {
                if (s.hero.state < 4 && !s.hero.abilities[1].disabled)
                {
                    if (s.hero.abilities[1].CDTimer <= 0)
				    {
					    s.abilityBeingCast = 2;
						s.hero.abilities[1].CDTimer = s.hero.abilities[1].cooldown;
						s.hero.photonView.RPC("Cast", RpcTarget.MasterClient, s.spawnPosition, 1);
                        BattleCardGroup.CardPlayed(s);
				    }
				    else
				    {
					    BattleWarning.Display(BattleWarningType.AbilityOnCooldown);
                    }
                }
                else
                {
	                BattleWarning.Display(BattleWarningType.HeroAlreadyCasting);
                }
            }
		}
		else
		{
			if (s.heroCD < 0)
			{
				Quaternion rotation = s.unitRotation.rotation;
				if (s.teamID == 1)
				{
					rotation = Quaternion.Inverse(s.unitRotation.rotation);
				}

                HeroUnit u = PhotonNetwork.Instantiate("Units/" + s.playerHero.starLevel + "/" + s.playerHero.Unit.gameObject.name, s.spawnPosition, rotation).GetComponent<HeroUnit>();
                if(Battle.instance.battleType != BattleType.LivePvP)
                {
                    u.photonView.RPC("SetRPC", RpcTarget.All, s.teamID, s.playerHero.level);
                }
                else
                {
                    u.photonView.RPC("SetRPC", RpcTarget.All, s.teamID, 1);
                }
                u.photonView.RPC("PlayerControlled", RpcTarget.All);

                s.hero = u;
                s.heroCD = ((HeroUnit)s.playerHero.Unit).cooldown;

                BattleCardGroup.CardPlayed(s);
			}
			else
			{
				BattleWarning.Display(BattleWarningType.HeroOnCooldown);
			}
		}
	}
}
