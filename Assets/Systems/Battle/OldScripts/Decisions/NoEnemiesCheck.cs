using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NoEnemiesCheck : UnitDecision
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

        if (u.targetInfo.targetUnit != null && u.targetInfo.distance > (u.aggroRange + u.radius))
        {
            u.targetInfo = abilityTarget;
            return true;
        }
        return false;
    }
}
