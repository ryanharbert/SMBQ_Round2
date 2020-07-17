using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShieldAction : UnitAction
{
    public GameObject displayObject;
    public int uses;

    int useCount = 0;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        u.photonView.RPC("AbilityDisplayBool", RpcTarget.All, type, index, false);
    }
    
    public override void BoolDisplay(bool active)
    {
        useCount += 1;
        if (displayObject != null)
        {
            displayObject.SetActive(false);
        }
        if(uses != -1 && useCount >= uses)
        {
            Destroy(this);
        }
    }
}
