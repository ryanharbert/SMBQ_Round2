using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class UnitDecision
{
    public virtual bool Check(BattleState s, Unit u, Targeting AI_Targeting, float AI_TriggerDistance)
    {
        return false;
    }
}
