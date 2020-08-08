using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChargeCheck : DistanceCheck
{
    public override bool Check(BattleState s, Unit u, Targeting AI_Targeting, float AI_TriggerDistance)
    {
        ChargeMove m = (ChargeMove)u.move;

        if (m.charging)
        {
            return base.Check(s, u, AI_Targeting, AI_TriggerDistance);
        }
        return false;
    }
}
