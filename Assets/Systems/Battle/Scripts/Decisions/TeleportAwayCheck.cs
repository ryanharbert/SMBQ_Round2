using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportAwayCheck : UnitDecision
{
    public override bool Check(BattleState s, Unit u, Targeting AI_Targeting, float AI_TriggerDistance)
    {
        if ((u.maxHealth * 0.6f) > u.health)
        {
            TargetInfo t = new TargetInfo();
            t = ClosestAlly(s, u, t);

            if ((AI_TriggerDistance == -1 || (AI_TriggerDistance + u.radius) > t.distance))
            {
                u.targetInfo = t;
                return true;
            }
        }
        return false;
    }

    TargetInfo ClosestAlly(BattleState s, Unit u, TargetInfo t)
    {
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] == null)
            {
                continue;
            }

            if (s.units[i].teamID == u.teamID && u != s.units[i])
            {
                float possibleDistance = Vector3.Distance(u.transform.position, s.units[i].transform.position) - s.units[i].radius;
                if (t.distance == 0 || possibleDistance < t.distance)
                {
                    t.distance = possibleDistance;
                    t.targetUnit = s.units[i];
                }
            }
        }
        return t;
    }
}
