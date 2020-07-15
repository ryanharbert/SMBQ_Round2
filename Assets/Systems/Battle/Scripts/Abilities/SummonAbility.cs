using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SummonAbility : AbilityType
{
	public CardData card;

    //AI
    public Transform AISpawnPosition;

	public override void Effect()
	{
		if(!PhotonNetwork.IsMasterClient)
			return;

		if(!triggered)
		{
			card.PlayCard(Battle.state, position, u.teamID, u.level);
			triggered = true;
		}
    }

    protected override bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, AISpawnPosition.position, index);
        return true;
    }
}
