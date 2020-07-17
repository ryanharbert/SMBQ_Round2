using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DistanceCheck : UnitDecision
{
    public override bool Check(BattleState s, Unit u, Targeting AI_Targeting, float AI_TriggerDistance)
    {
        TargetInfo abilityTarget = new TargetInfo();
        if (AI_Targeting == u.targeting || AI_TriggerDistance == -1)
        {
            abilityTarget = u.targetInfo;
        }
        else
        {
            abilityTarget = UnitSystem.ClosestEnemy(s, u, AI_Targeting);
        }

        if (abilityTarget.targetUnit != null && (AI_TriggerDistance == -1 || (AI_TriggerDistance + u.radius) > abilityTarget.distance))
        {
            u.targetInfo = abilityTarget;
            return true;
        }
        return false;
    }
}
