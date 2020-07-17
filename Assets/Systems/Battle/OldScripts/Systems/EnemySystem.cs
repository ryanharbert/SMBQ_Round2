using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySystem : BaseSystem
{
    Quaternion EnemyBuildingRotation(BattleState s, int i)
    {
        if(i == -1)
        {
            if (s.battleLayout.enemyStrongholdRotation != 0)
            {
                return Quaternion.Euler(new Vector3(0f, s.battleLayout.enemyStrongholdRotation));
            }
        }
        else
        {
            if(s.battleLayout.enemyBuildingRotations != null && s.battleLayout.enemyBuildingRotations.Length > i)
            {
                return Quaternion.Euler(new Vector3(0f, s.battleLayout.enemyBuildingRotations[i]));
            }
        }
        return Quaternion.Inverse(s.unitRotation.rotation);
    }

	public override void Init(BattleState s)
	{
        if (Battle.instance.battleType == BattleType.LivePvP)
            return;

		if (Battle.instance.battleType == BattleType.Raid || Battle.instance.battleType == BattleType.LiveRaid)
		{
			s.gameLength = 300;
		}
		else if (Battle.instance.battleType == BattleType.AsyncPvP)
		{
			s.gameLength = 360;
		}
		else
		{
			s.gameLength = 240;
		}
		s.gameTimer = 0;

		if (Battle.instance.battleType == BattleType.LiveRaid && !PhotonNetwork.LocalPlayer.IsMasterClient)
			return;

		if (Battle.instance.battleType != BattleType.AsyncPvP)
        {
            Unit u = PhotonNetwork.Instantiate("EnemyBuildings/" + s.enemyStronghold.name, s.battleLayout.enemyStrongholdLocation, EnemyBuildingRotation(s, -1)).GetComponent<Unit>();
            u.photonView.RPC("SetRPC", RpcTarget.All, 1, s.enemyLevel);
            s.enemyObjectives.Add(u);
            if(Battle.instance.battleType == BattleType.LiveRaid)
            {
                u.photonView.RPC("RaidBoss", RpcTarget.OthersBuffered);
            }

            for (int i = 0; i < s.enemyBuildings.Count; i++)
            {
                if(i < s.battleLayout.enemyBuildingLocations.Length)
                {
                    Unit b = PhotonNetwork.Instantiate("EnemyBuildings/" + s.enemyBuildings[i].name, s.battleLayout.enemyBuildingLocations[i], EnemyBuildingRotation(s, i)).GetComponent<Unit>();
                    b.photonView.RPC("SetRPC", RpcTarget.All, 1, s.enemyLevel);
                }
            }
        }
        else
        {
            Unit u = PhotonNetwork.Instantiate("Units/" + s.enemyStarData.stronghold + "/" + s.enemyStronghold.name, s.battleLayout.enemyStrongholdLocation, Quaternion.Inverse(s.unitRotation.rotation)).GetComponent<Unit>();
            u.photonView.RPC("SetRPC", RpcTarget.All, 1, s.enemyLevelData.stronghold);
            s.enemyObjectives.Add(u);
            u.PlayerNameAboveStronghold(Data.instance.pvpBattle.enemyName);
        }

		s.enemyCurrentMana = 7;
		s.enemyDeckIndex = 0;
		s.enemyHeroCD = 15;
	}

	public override void Run(BattleState s)
    {
        if (Battle.instance.battleType == BattleType.LivePvP)
            return;

        if (s.gameOver)
			return;

		if (s.gameTimer < s.gameLength)
		{
			s.gameTimer += Time.deltaTime;
		}

		if (Battle.instance.battleType == BattleType.LiveRaid && !PhotonNetwork.LocalPlayer.IsMasterClient)
			return;

		if (s.gameTimer < s.gameLength)
		{
			s.enemyManaProgress += Time.deltaTime * s.enemyManaRate;
		}
		else
		{
			s.enemyManaProgress += Time.deltaTime * s.enemyManaRate * 3;
		}

		if(s.enemyManaProgress >= 1)
		{
			s.enemyManaProgress = 0;
			s.enemyCurrentMana += 1;
		}

		if(s.enemyDeck[s.enemyDeckIndex].ManaCost <= s.enemyCurrentMana)
		{
            int level = 1;
            int star = 0;
            if (Battle.instance.battleType != BattleType.AsyncPvP)
            {
                level = s.enemyLevel;
                if (level > 29 && s.enemyDeck[s.enemyDeckIndex].starPowers.Length > 3)
                {
                    star = 3;
                }
                else if (level > 19 && s.enemyDeck[s.enemyDeckIndex].starPowers.Length > 2)
                {
                    star = 2;
                }
                else if (level > 9 && s.enemyDeck[s.enemyDeckIndex].starPowers.Length > 1)
                {
                    star = 1;
                }
            }
            else
            {
                level = s.enemyLevelData.deck[s.enemyDeckIndex];
                star = s.enemyStarData.deck[s.enemyDeckIndex];
            }
			s.enemyDeck[s.enemyDeckIndex].PlayCard(s, s.battleLayout.spawnPositions[Random.Range(0, s.battleLayout.spawnPositions.Length)], 1, level, star);
			s.enemyCurrentMana -= s.enemyDeck[s.enemyDeckIndex].ManaCost;
			if(s.enemyDeckIndex < (s.enemyDeck.Count - 1))
			{
				s.enemyDeckIndex++;
			}
			else
			{
				s.enemyDeckIndex = 0;
			}
		}


		if (Battle.instance.battleType == BattleType.AsyncPvP)
		{
			if(s.enemyHeroCD < 0 && s.enemyHero == null)
			{
				Quaternion rotation = s.unitRotation.rotation;
				rotation = Quaternion.Inverse(s.unitRotation.rotation);
				
				string enemyHeroName = Data.instance.collection.allCards[Data.instance.pvpBattle.enemyHero].Unit.gameObject.name;
				HeroUnit u = PhotonNetwork.Instantiate("Units/" + s.enemyStarData.hero + "/" + enemyHeroName, s.battleLayout.spawnPositions[Random.Range(0, s.battleLayout.spawnPositions.Length)], rotation).GetComponent<HeroUnit>();
				u.photonView.RPC("SetRPC", Photon.Pun.RpcTarget.All, 1, s.enemyLevelData.hero);
				s.enemyHero = u;
				s.enemyHeroCD = s.enemyHero.cooldown;
			}
			else
			{
				s.enemyHeroCD -= Time.deltaTime;
			}
		}
	}
}
