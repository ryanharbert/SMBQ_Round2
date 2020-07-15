using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LowHealthAllyCheck : UnitDecision
{
    public override bool Check(BattleState s, Unit u, Targeting AI_Targeting, float AI_TriggerDistance)
    {
        TargetInfo t = new TargetInfo();
        t = ClosestLowHealthAlly(s, u, t);

        if ((AI_TriggerDistance == -1 || (AI_TriggerDistance + u.radius) > t.distance))
        {
            u.targetInfo = t;
            return true;
        }
        return false;
    }

    TargetInfo ClosestLowHealthAlly(BattleState s, Unit u, TargetInfo t)
    {
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] == null)
            {
                continue;
            }

            if ((s.units[i].maxHealth * 0.5f) > s.units[i].health)
            {
                if (s.units[i].teamID == u.teamID)
                {
                    float possibleDistance = Vector3.Distance(u.transform.position, s.units[i].transform.position) - s.units[i].radius;
                    if (t.distance == 0 || possibleDistance < t.distance)
                    {
                        t.distance = possibleDistance;
                        t.targetUnit = s.units[i];
                        t.position = t.targetUnit.transform.position;
                    }
                }
            }
        }
        return t;
    }
}
