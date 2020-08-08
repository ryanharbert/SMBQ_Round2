using System.Collections;
using System.Collections.Generic;
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
            u.PlayerNameAboveStronghold(Data.instance.user.displayName);
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
            BattleCard b = s.selectedCard;

			s.selectedCard = Battle.instance.battleCardGroup.battleCards[0];
			s.spawnPosition = s.battleLayout.playerAutoSpawnPositions[Random.Range(0, s.battleLayout.playerAutoSpawnPositions.Length)];
			s.currentMana -= s.playerDeck[s.selectedCard.index].ManaCost;
			s.playerDeck[s.selectedCard.index].PlayCard(s, s.spawnPosition, s.teamID, s.playerDeck[s.selectedCard.index].level);

			SendCardToBack(s);
			if (b != null)
			{
				s.selectedCard = b;
			}
		}
		else if(s.playCard)
		{
			s.playCard = false;
            if(s.selectedCard != null)
            {
                if (s.selectedCard is BattleAbility && s.hero[((BattleAbility)s.selectedCard).hero.index] != null)
                {
                    UseAbility(s, ((BattleAbility)s.selectedCard).hero.index, ((BattleAbility)s.selectedCard).index);
                }
                else if(s.selectedCard is BattleHero)
                {
                    SummonHero(s, ((BattleHero)s.selectedCard).index);
                }
                else
                {
                    if (s.playerDeck[s.selectedCard.index].ManaCost <= s.currentMana)
                    {
                        s.currentMana -= s.playerDeck[s.selectedCard.index].ManaCost;
                        if (Battle.instance.battleType != BattleType.LivePvP)
                        {
                            s.playerDeck[s.selectedCard.index].PlayCard(s, s.spawnPosition, s.teamID, s.playerDeck[s.selectedCard.index].level);
                        }
                        else
                        {
                            int star = 0;
                            if (s.playerDeck[s.selectedCard.index].starPowers.Length > 3)
                            {
                                star = 3;
                            }
                            else if (s.playerDeck[s.selectedCard.index].starPowers.Length > 2)
                            {
                                star = 2;
                            }
                            else if (s.playerDeck[s.selectedCard.index].starPowers.Length > 1)
                            {
                                star = 1;
                            }
                            s.playerDeck[s.selectedCard.index].PlayCard(s, s.spawnPosition, s.teamID, 1, star);
                        }

                        SendCardToBack(s);
                    }
                    else
                    {
                        DisplayWarning(BattleWarning.NotEnoughMana);
                    }
                }
            }
            else
            {
                DisplayWarning(BattleWarning.NoCardSelected);
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
        CardData c = s.playerDeck[s.selectedCard.index];

        for (var i = 3; i < s.playerDeck.Count; i++)
        {
            if (i == 3)
            {
                s.playerDeck[s.selectedCard.index] = s.playerDeck[i];
				if(s.autoPlay && s.selectedCard.index == 0)
				{
					Battle.instance.battleCardGroup.battleCards[0].cardDisplay.SetCardDisplay(s.playerDeck[s.selectedCard.index].itemID);
					Battle.instance.battleCardGroup.battleCards[0].cardData = s.playerDeck[s.selectedCard.index];
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

    void UseAbility(BattleState s, int heroIndex, int abilityIndex)
    {
        if (s.hero[heroIndex].state < 4)
        {
            if (s.hero[heroIndex].abilities[abilityIndex].CDTimer <= 0 && !s.hero[heroIndex].abilities[abilityIndex].disabled)
            {
                s.hero[heroIndex].abilities[abilityIndex].CDTimer = s.hero[heroIndex].abilities[abilityIndex].cooldown;
                s.hero[heroIndex].photonView.RPC("Cast", RpcTarget.MasterClient, s.spawnPosition, abilityIndex);
                BattleCardGroup.CardPlayed(s);
            }
            else
            {
                DisplayWarning(BattleWarning.AbilityOnCooldown);
            }
        }
        else
        {
            DisplayWarning(BattleWarning.HeroAlreadyCasting);
        }
    }

    void SummonHero(BattleState s, int heroIndex)
    {
        if (s.heroCD[heroIndex] > 0)
        {
            DisplayWarning(BattleWarning.HeroOnCooldown);
        }
        else if (s.playerHero[heroIndex].ManaCost > s.currentMana)
        {
            DisplayWarning(BattleWarning.NotEnoughMana);
        }
        else
        {
            Quaternion rotation = s.unitRotation.rotation;
            if (s.teamID == 1)
            {
                rotation = Quaternion.Inverse(s.unitRotation.rotation);
            }

            HeroUnit u = PhotonNetwork.Instantiate("Units/" + s.playerHero[heroIndex].starLevel + "/" + s.playerHero[heroIndex].Unit.gameObject.name, s.spawnPosition, rotation).GetComponent<HeroUnit>();
            if (Battle.instance.battleType != BattleType.LivePvP)
            {
                u.photonView.RPC("SetRPC", RpcTarget.All, s.teamID, s.playerHero[heroIndex].level);
            }
            else
            {
                u.photonView.RPC("SetRPC", RpcTarget.All, s.teamID, 1);
            }
            u.photonView.RPC("PlayerControlled", RpcTarget.All);

            s.hero[heroIndex] = u;
            s.heroCD[heroIndex] = ((HeroUnit)s.playerHero[heroIndex].Unit).cooldown;
            s.currentMana -= s.playerHero[heroIndex].ManaCost;

            BattleCardGroup.CardPlayed(s);
        }
    }

	//WARNINGS
	void DisplayWarning(BattleWarning warningType)
	{
		CancelInvoke();
		//warningText.transform.position = Input.mousePosition;


		switch (warningType)
		{
			case BattleWarning.NotEnoughMana:
				Battle.instance.warningText.text = "Not Enough Mana";
				break;
			case BattleWarning.NoCardSelected:
				Battle.instance.warningText.text = "No Card Selected";
				break;
			case BattleWarning.HeroOnCooldown:
				Battle.instance.warningText.text = "Hero is on Cooldown";
				break;
			case BattleWarning.AbilityOnCooldown:
				Battle.instance.warningText.text = "Hero's Ability is on Cooldown";
				break;
			case BattleWarning.HeroAlreadyCasting:
				Battle.instance.warningText.text = "Hero is Casting another Ability";
				break;
		}
		
		Invoke("ClearWarningText", 0.5f);
	}

	void ClearWarningText()
	{
		Battle.instance.warningText.text = "";
	}
}
