using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AddBuffAction : UnitAction
{
    public UnitBuff buff;
    public bool onSelf;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        if(onSelf)
        {
            u.photonView.RPC("AddBuff", RpcTarget.All, buff.name);
        }
        else
        {
            t.targetUnit.photonView.RPC("AddBuff", RpcTarget.All, buff.name);
        }
    }
}
