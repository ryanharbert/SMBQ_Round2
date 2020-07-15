using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MultiActionAction : UnitAction
{
    public UnitAction[] actions;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        for(int i = 0; i < actions.Length; i++)
        {
            actions[i].Trigger(u, t, type, index);
        }
    }
}
