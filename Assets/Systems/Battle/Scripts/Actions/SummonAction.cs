using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SummonAction : UnitAction
{
    public CardData card;
    public Transform summonLocation;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        if (!u.AI || summonLocation == null)
        {
            card.PlayCard(Battle.state, t.position, u.teamID, u.level);
        }
        else
        {
            card.PlayCard(Battle.state, summonLocation.position, u.teamID, u.level);
        }
    }
}
